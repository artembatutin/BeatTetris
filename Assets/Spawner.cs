using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    //block groups
    public GameObject[] groups;
    public SpriteRenderer panel;

    //game camera
    public new Camera camera;

    //audio
    public new AudioSource audio;
    public static float clipLoudness;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;
    private float currentUpdateTime = 0f;
    private float[] clipSampleData;

    public float r, g, b;
    public int tetrisTimer;

    public void SpawnNext() {
        int i = Random.Range(0, groups.Length);
        GameObject o = Instantiate(groups[i], transform.position, Quaternion.identity);
        SpriteRenderer sr = o.GetComponentInChildren<SpriteRenderer>();
        Grid.ColorBorders(sr.color);
        r = sr.color.r / 10;
        g = sr.color.g / 10;
        b = sr.color.b / 10;
    }

    void Update() {
        if(!Grid.playing && Input.GetKeyDown(KeyCode.Space)) {
            Grid.Restart();
            SpawnNext();
            return;
        }
        if(Input.GetKeyDown(KeyCode.T)) {
            Grid.tetris = true;
        }
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
        panel.color = new Color(clipLoudness * 3, clipLoudness * 3, clipLoudness * 3);
        if(Grid.tetris) {
            tetrisTimer = 500;
            audio.time = 26.7f;
            Grid.tetris = false;
        }
        if (tetrisTimer > 0) {
            tetrisTimer--;
            if (tetrisTimer == 0) {
                panel.color = new Color(1, 1, 1);
                Grid.title.color = new Color(1, 1, 1);
				foreach (Transform o in Group.container.transform) {
					float z = o.transform.eulerAngles.z;
					o.transform.eulerAngles = new Vector3(0, 0, z);
				}
            } else {
                panel.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                Grid.title.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                if(Group.container != null) {
                    foreach(Transform o in Group.container.transform) {
                        Group g = o.GetComponent<Group>();
						float rotc = Random.Range(0, Spawner.clipLoudness * 10);
						float rot = Random.Range(0, 2) == 1 ? -rotc : rotc;
                        o.transform.eulerAngles = new Vector3(0, 0, g.GetRotation() + rot);
                    }
                }
            }
            camera.backgroundColor = new Color(Random.Range(0f, 0.4f) + clipLoudness, Random.Range(0f, 0.4f) + clipLoudness, Random.Range(0f, 0.4f) + clipLoudness);
        } else {
            camera.backgroundColor = new Color(r + clipLoudness, g + clipLoudness, b + clipLoudness);
        }
    }

    void Start() {
        clipSampleData = new float[sampleDataLength];
        Grid.Start();
        SpawnNext();
    }
}
