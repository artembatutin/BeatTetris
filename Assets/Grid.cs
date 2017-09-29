using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour {

    public static bool playing = true;

    public static int w = 10;
    public static int h = 20;
    public static Transform[,] grid = new Transform[w, h];

    public static bool tetris;
    public static Text title;
    public static Text gameText;

    private static int score = 0;
    private static Text scoreText;
    private static int level = 1;
    private static Text levelText;

    private static SpriteRenderer firstLine;
    private static SpriteRenderer secondLine;

    public static void Start() {
        title = GameObject.Find("Tetris").GetComponent<Text>();
		gameText = GameObject.Find("GameText").GetComponent<Text>();
        scoreText = GameObject.Find("ScoreNum").GetComponent<Text>();
        levelText = GameObject.Find("LevelNum").GetComponent<Text>();
        firstLine = GameObject.Find("LeftBorder").GetComponent<SpriteRenderer>();
        secondLine = GameObject.Find("RightBorder").GetComponent<SpriteRenderer>();
    }

    public static void Restart() {
		Destroy(Group.container);
        GameObject o = new GameObject("Blocks");
        Group.container = o;
        score = 0;
        scoreText.text = score.ToString();
        level = 1;
        levelText.text = level.ToString();
        tetris = false;
        playing = true;
        gameText.text = "";
        grid = new Transform[w, h];
    }

    public static void ColorBorders(Color color) {
        firstLine.color = color;
        secondLine.color = color;
    }

    public static void DeleteRow(int y) {
        for (int x = 0; x < w; ++x) {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
        score++;
        if (score % 10 == 0) {
            level++;
            levelText.text = level.ToString();
        }
        scoreText.text = score.ToString();
    }

    public static void decreaseRow(int y) {
        for (int x = 0; x < w; ++x) {
            if (grid[x, y] != null) {
                // Move one towards bottom
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                // Update Block position
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public static void DeleteFullRows() {
        int deleted = 0;
        for (int y = 0; y < h; ++y) {
            if (IsRowFull(y)) {
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                deleted++;
                --y;
            }
        }
        if(deleted >= 4) {
			level++;
			levelText.text = level.ToString();
            tetris = true;
        }
    }

    public static bool IsRowFull(int y) {
		for (int x = 0; x < w; ++x) {
			if (grid[x, y] == null) {
				return false;
			}
		}
		return true;
	}

	public static void DecreaseRowsAbove(int y) {
		for (int i = y; i < h; ++i) {
			decreaseRow(i);
		}
	}

    public static bool Valid(int x, int y) {
        return (x >= 0 && x < w && y >= 0);
    }

    public static bool IsOccupied(int x, int y) {
        if (x < 0 || y < 0 || x >= w || y >= h)
            return false;
        return grid[x, y] != null;
    }
}
