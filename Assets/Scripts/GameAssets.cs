using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// House assets
// Obstacles, etc.

public class GameAssets : MonoBehaviour {
	
	private int score = 0;
    public Text txt;
    public Text health;
    public Text highScore;
	private bool recentImpact = false;

    private int Alive=3;

    public static GameAssets instance;
	
	public static GameAssets GetInstance() {
		return instance;
	}

	public void Awake() {
		instance = this;
	}

    public void Start() {
        health.text = "Health: " + Alive;
        highScore.text = "High Score: " + PlayerPrefs.GetInt("highScore");
    }
	
	// Stores assets for obstacles
	public Transform jumpObsBody;
    public Transform jumpObs2Body;
    public Transform duckObsBody;
    public Transform duckObs2Body;
    public Transform diveObsBody;
    public Transform bgBody;
    public Transform mgBody;
    public Transform fgBody;
    public Transform newGroundBody;

    public void increaseScore() {
		if (recentImpact == false) {
			score++;
			Debug.Log("Current Score: " + score);
			txt.text = "Current Score: " + score;
            if (score > PlayerPrefs.GetInt("highScore")) {
                highScore.text = "High Score: " + score;
            }
		}
    }
	
	public int getScore() {
		return score;
	}
	
	public void resetScore() {
		score = 0;
        
		Debug.Log("Current Score: " + score);
        txt.text = "Current Score: " + score;
       
    }
    public int reducehealth()
    {
        Alive -= 1;
        health.text = "Health: " + Alive;
        return Alive;

    }
}
