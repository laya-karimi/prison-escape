using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

	private const float OBSTACLE_SPEED = 5f; // Sets speed of obstacles moving towards player
	private const float OBSTACLE_DESTROY_POSITION = -15f; // x Position past player where Obstacles get destroyed and score increases
	private const float ENEMY_START_POSITION = 14f;
	private const int JUMP_OBSTACLE = 1;
    private const int JUMP_OBSTACLE2 = 4;
    private const int DIVE_OBSTACLE = 2;
	private const int DUCK_OBSTACLE = 3;
    private const int DUCK_OBSTACLE2 = 5;
    private const float FOREGROUND_SPEED = 3f; //Sets the speed of the background elements
    private const float MIDGROUND_SPEED = 2f; //Sets the speed of the midground elements
    private const float BACKGROUND_SPEED = 1f; //Sets the speed of the background elements

    //Create two objects of each to act as a double buffer
    private Transform mg1Transform;
    private Transform mg2Transform;
    private Transform bg1Transform;
    private Transform bg2Transform;
    private Transform fg1Transform;
    private Transform fg2Transform;
    private Transform ngTransform;
    private Transform ng2Transform;

    private List<Obstacle> obstacleList;
	private float spawnTimer;
	private float spawnTimerMax;
	private State state;
	
	public enum Difficulty {
		Easy,
		Easier,
		Medium,
		Hard,
		Hardest,
	}
	
	private enum State {
		Playing,
		Dead,
	}
	
	private void Awake() {
		obstacleList = new List<Obstacle>();
		//spawnTimerMax = 1f;
		SetDifficulty(Difficulty.Easy);
		state = State.Playing;
	}
	
	private void Start() {
        CreateBackground();
		PlayerControl.GetInstance().OnDeath += PlayerControl_OnDeath;
    }
	
	private void PlayerControl_OnDeath(object sender, System.EventArgs e) {
		Debug.Log("Death!");
		PlayerPrefs.SetInt("highScore", GameAssets.GetInstance().getScore());
		state = State.Dead;
	}

	private void Update() {
		if (state == State.Playing) {
			ObstacleMovement();
			BackgroundMovement();
			Spawner();
		}
	}
	
	private void Spawner() {
		spawnTimer -= Time.deltaTime;
		if (spawnTimer < 0) {
			//if true, spawn a new obstacle
			spawnTimer += spawnTimerMax;
			spawnObstacle(ENEMY_START_POSITION);
		}
	}

    private void CreateBackground() {
        //Creates the background objects and positions them on the scene
        mg2Transform = Instantiate(GameAssets.GetInstance().mgBody);
        mg1Transform = Instantiate(GameAssets.GetInstance().mgBody);
        bg1Transform = Instantiate(GameAssets.GetInstance().bgBody);
        bg2Transform = Instantiate(GameAssets.GetInstance().bgBody);
        fg2Transform = Instantiate(GameAssets.GetInstance().fgBody);
        fg1Transform = Instantiate(GameAssets.GetInstance().fgBody);
        ngTransform = Instantiate(GameAssets.GetInstance().newGroundBody);
        ng2Transform = Instantiate(GameAssets.GetInstance().newGroundBody);

        mg2Transform.position = new Vector3(32.8f, 3.8f);
        mg1Transform.position = new Vector3(-0.4f, 3.8f);
        bg1Transform.position = new Vector3(5f, 3.5f);
        bg2Transform.position = new Vector3(39f, 3.5f);
        fg1Transform.position = new Vector3(2.6f, 0f, 30f);
        fg2Transform.position = new Vector3(23f, 0f, 30f);
        ngTransform.position = new Vector3(-0.24f, -5.72f, 3f);
        ng2Transform.position = new Vector3(20f, -5.72f, 3f);
    }

	private void ObstacleMovement() {
		for (int i = 0; i < obstacleList.Count; i++) {
			Obstacle obstacle = obstacleList[i];
			bool toRightOfPlayer = obstacle.getXPos() > PlayerControl.GetInstance().GetPlayerPos();
			obstacle.move();
	
			if (toRightOfPlayer && obstacle.getXPos() <= PlayerControl.GetInstance().GetPlayerPos()) {
                // Score track and printer
                GameAssets.GetInstance().increaseScore();
            }
	
			// Destory obstacles if player dodges them and they move too far to the left
			if (obstacle.getXPos() < OBSTACLE_DESTROY_POSITION) {


				// Destroy Obstacle
				obstacle.selfDestruct();
				Debug.Log("Obstacle Destroyed");
				
				// Remove object from list and decrement index so that we do not skip
				// any obstacles in the list above when one gets destroyed
				obstacleList.Remove(obstacle);
				i--;
			}
		}
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

    // This method spawns an obstacle
    private void spawnObstacle(float xPos) {
		
		SetDifficulty(GetDifficulty());
		int obstacleType  = Random.Range(1, 6);
		Debug.Log("Current Difficulty: " + GetDifficulty());	
		
        // Obstacles to jump over
        if (obstacleType == 1 || obstacleType == 4) {
            Transform jumpObstacle;
            float yPos;
            bool running;
            if (obstacleType ==1)
            {
                jumpObstacle = Instantiate(GameAssets.GetInstance().jumpObsBody);
                yPos = -3.5f;
                running = false;
            }
            else
            {
                jumpObstacle = Instantiate(GameAssets.GetInstance().jumpObs2Body);
                yPos = -4f;
                running = true;
            }

            jumpObstacle.position = new Vector3(xPos, yPos); // Initial position for obstacle
			obstacleList.Add(new Obstacle(jumpObstacle, running));
		}

		
		// Obstacles to dive through
		if (obstacleType == 2) {
            Transform diveObstacle = Instantiate(GameAssets.GetInstance().diveObsBody);
            diveObstacle.position = new Vector3(xPos, -1.7f); // Initial position for obstacle
            obstacleList.Add(new Obstacle(diveObstacle, false));
			if (GetDifficulty() == Difficulty.Hard || GetDifficulty() == Difficulty.Hardest)
				spawnTimer += 1f;
		}
		
		// Obstacles to duck under
		if (obstacleType == 3 || obstacleType == 5) {
            Transform duckObstacle;
            float yPos;
            bool moving;
            if(obstacleType ==3)
            {
                duckObstacle = Instantiate(GameAssets.GetInstance().duckObsBody);
                yPos = -3.5f;
                moving = false;
            }
            else
            {
                duckObstacle = Instantiate(GameAssets.GetInstance().duckObs2Body);
                yPos = 0f;
                moving = true;
            }
            
			//duckObstacle.position = new Vector3(xPos, -2.0f); // Initial position for obstacle
            duckObstacle.position = new Vector3(xPos, yPos); // Initial position for obstacle
            obstacleList.Add(new Obstacle(duckObstacle, moving));
		}
	}
	
	// Class to store each obstacle on the screen 
	private class Obstacle {
		
		private Transform obstacle;
        private bool running; //check if obstacle is a moving obstacle to determine if it should move faster than ground
		
		public Obstacle(Transform t, bool r) {
			this.obstacle = t;
            this.running = r;
		}
		
		public void move() {
            if(running == false)
			    obstacle.position += new Vector3(-1, 0, 0) * OBSTACLE_SPEED * Time.deltaTime;
            else
                obstacle.position += new Vector3(-1, 0, 0) * (OBSTACLE_SPEED * 1.5f ) * Time.deltaTime; //makes object move faster than ground
        }
		
		public float getXPos() {
			return obstacle.position.x;
		}

        public float getYPos(){
            return obstacle.position.y;
        }

        public void selfDestruct() {
			Destroy(obstacle.gameObject);
		}
	}
	
	// Speed between obstacle respawns
	private void SetDifficulty(Difficulty diff) {
		switch(diff){
		case(Difficulty.Easy):
			spawnTimerMax = 2.7f;
			break;
		case(Difficulty.Easier):
			spawnTimerMax = 2.4f;
			break;
		case(Difficulty.Medium):
			spawnTimerMax = 2.1f;
			break;
		case(Difficulty.Hard):
			spawnTimerMax = 1.8f;
			break;
		case(Difficulty.Hardest):
			spawnTimerMax = 1.5f;
			break;
		}
	}
	
	// Score required to increase difficulty
	private Difficulty GetDifficulty() {
		Debug.Log("GameAssets.GetInstance().getScore() = " + GameAssets.GetInstance().getScore());
		
		if (GameAssets.GetInstance().getScore() >= 35)
			return Difficulty.Hardest;
		if (GameAssets.GetInstance().getScore() >= 25)
			return Difficulty.Hard;
		if (GameAssets.GetInstance().getScore() >= 15)
			return Difficulty.Medium;
		if (GameAssets.GetInstance().getScore() >= 5)
			return Difficulty.Easier;
		return Difficulty.Easy;

		
			
	}
	
		
}


