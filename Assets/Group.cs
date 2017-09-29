using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour {

    public static GameObject container;
    public static Spawner spawner;

    private bool fallDown;
    private float lastFall = 0;
    private float rotation;

    // Use this for initialization
    void Start() {
        if (!IsValidSpawn()) {
            Grid.playing = false;
            Grid.gameText.text = "You're a LOOSER (Space to restart)";
            Destroy(gameObject);
        }
        if(spawner == null)
            spawner = GameObject.Find("Spawner").GetComponent<Spawner>();
		if (container == null)
			container = GameObject.Find("Blocks");
		if (container != null)
			transform.parent = container.transform;
    }

    // Update is called once per frame
    void Update() {
        float rotc = Random.Range(0, Spawner.clipLoudness * 10);
        float rot = Random.Range(0, 2) == 1 ? -rotc : rotc;
        transform.eulerAngles = new Vector3(0, 0, rotation + rot);
		if (fallDown || Time.time - lastFall >= 1 || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
			// Modify position
			transform.position += new Vector3(0, -1, 0);

			// See if valid
			if (!IsValidGridPos()) {
				// It's not valid. revert.
				transform.position += new Vector3(0, 1, 0);
                transform.eulerAngles = new Vector3(0, 0, rotation);
				// Clear filled horizontal lines
				Grid.DeleteFullRows();
				// Spawn next Group
				spawner.SpawnNext();
				// Disable script
				enabled = false;
			}
            UpdateGrid();
			lastFall = Time.time;
		}
        // Move Left
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            // Modify position
            transform.position += new Vector3(-1, 0, 0);

            // See if valid
            if (IsValidGridPos())
                // It's valid. Update grid.
                UpdateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(1, 0, 0);
        }

        // Move Right
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            // Modify position
            transform.position += new Vector3(1, 0, 0);

            // See if valid
            if (IsValidGridPos())
                // It's valid. Update grid.
                UpdateGrid();
            else
                // It's not valid. revert.
                transform.position += new Vector3(-1, 0, 0);
        }

        // Rotate
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            rotation -= 90;
            transform.eulerAngles = new Vector3(0, 0, rotation);
            // See if valid
            if (IsValidGridPos())
                // It's valid. Update grid.
                UpdateGrid();
            else {
                // It's not valid. revert.
                rotation += 90;
                transform.eulerAngles = new Vector3(0, 0, rotation);
            }
        }

        // Move Downwards and Fall
        else if(Input.GetKeyDown(KeyCode.Space)) {
            fallDown = true;
        }
    }

    void UpdateGrid() {
        // Remove old children from grid
        for (int y = 0; y < Grid.h; ++y)
            for (int x = 0; x < Grid.w; ++x)
                if (Grid.grid[x, y] != null)
                    if (Grid.grid[x, y].parent == transform)
                        Grid.grid[x, y] = null;

        // Add new children to grid
        foreach (Transform child in transform) {
			int x = (int)Mathf.Round(child.position.x);
			int y = (int)Mathf.Round(child.position.y);
            Grid.grid[x, y] = child;
        }
    }
    bool IsValidSpawn() {
		foreach (Transform child in transform) {
            int x = (int)Mathf.Round(child.position.x);
            int y = (int)Mathf.Round(child.position.y);
			if (Grid.IsOccupied(x, y))
				return false;
		}
		return true;
    }

    bool IsValidGridPos() {
        foreach (Transform child in transform) {
			int x = (int)Mathf.Round(child.position.x);
			int y = (int)Mathf.Round(child.position.y);
            if (!Grid.Valid(x, y))
                return false;
            // Block in grid cell (and not part of same group)?
            if (Grid.grid[x, y] != null && Grid.grid[x, y].parent != transform)
                return false;
        }
        return true;
    }

    public float GetRotation() {
        return rotation;
    }
}
