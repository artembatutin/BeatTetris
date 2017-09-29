using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject[] groups;

    [SerializeField]
    public Camera camera;

    [SerializeField]
    public AudioSource audio;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    public float r, g, b;

    public void spawnNext() {
        int i = Random.Range(0, groups.Length);
        GameObject o = Instantiate(groups[i], transform.position, Quaternion.identity);
        SpriteRenderer sr = o.GetComponentInChildren<SpriteRenderer>();
        Grid.ColorBorders(sr.color);
        r = sr.color.r / 10;
        g = sr.color.g / 10;
        b = sr.color.b / 10;
    }

    void Update() {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep) {
            currentUpdateTime = 0f;
            audio.clip.GetData(clipSampleData, audio.timeSamples);
            clipLoudness = 0f;
            foreach (var sample in clipSampleData) {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength;
        }
        clipLoudness *= 1.2f;
        //camera.backgroundColor = new Color(clipLoudness, clipLoudness, clipLoudness);
        Color color = new Color(r + clipLoudness, g + clipLoudness, b + clipLoudness);
        camera.backgroundColor = color;
    }

    void Start() {
        clipSampleData = new float[sampleDataLength];
        Grid.Start();
        spawnNext();
    }
}
