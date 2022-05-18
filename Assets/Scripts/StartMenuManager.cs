using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{

    public Text highScore;

	private const float OBSTACLE_SPEED = 5f; // Sets speed of obstacles moving towards player
    private const float FOREGROUND_SPEED = 3f; //Sets the speed of the background elements
    private const float MIDGROUND_SPEED = 2f; //Sets the speed of the midground elements
    private const float BACKGROUND_SPEED = 1f; //Sets the speed of the background elements
	private Transform mg1Transform;
    private Transform mg2Transform;
    private Transform bg1Transform;
    private Transform bg2Transform;
    private Transform fg1Transform;
    private Transform fg2Transform;
    private Transform ngTransform;
    private Transform ng2Transform;

	public void Start() {
		CreateBackground();
        highScore.text = "High Score: " + PlayerPrefs.GetInt("highScore");
    }

    public void StartGame() {
		SceneManager.LoadScene("Main");
	}
	
	private void Update()
    {
        BackgroundMovement();
    }
	
	public Transform bgBody;
    public Transform mgBody;
    public Transform fgBody;
    public Transform newGroundBody;
	
	private void CreateBackground() {
        //Creates the background objects and positions them on the scene
        mg2Transform = Instantiate(mgBody);
        mg1Transform = Instantiate(mgBody);
        bg1Transform = Instantiate(bgBody);
        bg2Transform = Instantiate(bgBody);
        fg2Transform = Instantiate(fgBody);
        fg1Transform = Instantiate(fgBody);
        ngTransform = Instantiate(newGroundBody);
        ng2Transform = Instantiate(newGroundBody);

        mg2Transform.position = new Vector3(32.8f, 3.8f);
        mg1Transform.position = new Vector3(-0.4f, 3.8f);
        bg1Transform.position = new Vector3(5f, 3.5f);
        bg2Transform.position = new Vector3(39f, 3.5f);
        fg1Transform.position = new Vector3(2.6f, 0f, 30f);
        fg2Transform.position = new Vector3(23f, 0f, 30f);
        ngTransform.position = new Vector3(-0.24f, -5.72f, 3f);
        ng2Transform.position = new Vector3(20f, -5.72f, 3f);
    }
	
	private void BackgroundMovement() {
        //moves the sprites across the screen
        mg1Transform.position += new Vector3(-1, 0, 0) * MIDGROUND_SPEED * Time.deltaTime;
        mg2Transform.position += new Vector3(-1, 0, 0) * MIDGROUND_SPEED * Time.deltaTime;
        bg1Transform.position += new Vector3(-1, 0, 0) * BACKGROUND_SPEED * Time.deltaTime;
        bg2Transform.position += new Vector3(-1, 0, 0) * BACKGROUND_SPEED * Time.deltaTime;
        fg1Transform.position += new Vector3(-1, 0, 0) * FOREGROUND_SPEED * Time.deltaTime;
        fg2Transform.position += new Vector3(-1, 0, 0) * FOREGROUND_SPEED * Time.deltaTime;
        ngTransform.position += new Vector3(-1, 0, 0) * OBSTACLE_SPEED * Time.deltaTime;
        ng2Transform.position += new Vector3(-1, 0, 0) * OBSTACLE_SPEED * Time.deltaTime;

        //checks if sprites are off screen and resets their position if they are
        if (mg1Transform.position.x < -27f)
            mg1Transform.position = new Vector2(32.8f, 3.8f);
        if (mg2Transform.position.x < -27f)
            mg2Transform.position = new Vector2(32.8f, 3.8f);
        if (bg1Transform.position.x < -27f)
            bg1Transform.position = new Vector2(39f, 3.5f);
        if (bg2Transform.position.x < -27f)
            bg2Transform.position = new Vector2(39f, 3.5f);
        if (fg1Transform.position.x < -19.8f)
            fg1Transform.position = new Vector3(21f, 0f, 30f);
        if (fg2Transform.position.x < -19.8f)
            fg2Transform.position = new Vector3(21f, 0f, 30f);
        if (ngTransform.position.x < -19.5f)
            ngTransform.position = new Vector3(21f, -5.72f, 3f);
        if (ng2Transform.position.x < -19.5f)
            ng2Transform.position = new Vector3(21f, -5.72f, 3f);
    }
	
}
