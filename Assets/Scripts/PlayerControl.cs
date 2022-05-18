using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {
    
    // Player Config
    public float jumpVelocity = 8f;
    public float fallMultiplier = 2.5f;
    public float diveFloatTime = .5f;
    public float fallBackHealth = -3f;
    public int maxHealth = 3;
    // Collision / Body
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    // Player Status
    private bool isJumping = false;
    private bool isDucking = false;
    private bool isDiving = false;
    private int health;
    private int healthLocation = -1;
    private float startDive;
    // Animation
    private Animator animator;
    //Sound
    AudioSource[] JumpSound;
    AudioSource YellSound;
	
	// Death
	public event EventHandler OnDeath;
	
	private static PlayerControl instance;
	
	public static PlayerControl GetInstance() {
		return instance;
	}
	
	private void Awake() {
		instance = this;
	}
    
    private void Start() {
        body = GetComponent<Rigidbody2D> ();
        boxCollider = GetComponent<BoxCollider2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("Jump", false);
        animator.SetBool("Ducking", false);
        animator.SetBool("Landed", true);
        health = maxHealth;
        //JumpSound = GetComponent<AudioSource>();
        JumpSound = GetComponents<AudioSource>();
        YellSound = GetComponent<AudioSource>();
    }

    void Update() {
        // Keys
        bool jumpKey = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool duckKey = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        bool diveKey = Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown((KeyCode.D));
        // Jump Logic
        if (jumpKey && !isJumping && !isDiving) {
            handleJump();
        }
        // Improved Jump
        if (body.velocity.y < 0) { // Falling
            body.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        } else if (body.velocity.y > 0 && !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))) { // Long Jump
            body.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
        }
        // Dive Logic
        if (diveKey && !isDiving && !isDucking && !isJumping) {
            handleDive();
        }
        if (isDiving && body.velocity.y < 0 && (startDive + diveFloatTime) > Time.realtimeSinceStartup) {
            body.velocity = new Vector2(0,.2f);
        }
        // Duck Logic
        if (duckKey && !isDucking && !isDiving) {
            handleDuck();
        }
        // Finish Ducking
        if (Input.GetKeyUp(KeyCode.DownArrow) && isDucking || Input.GetKeyUp(KeyCode.S) && isDucking) {
            boxCollider.size = new Vector2(boxCollider.size.x,boxCollider.size.y * 4);
            animator.SetBool("Ducking", false);
            isDucking = false;
        } 
        updatePlayerLocationBasedOnHealth(false);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.name.Equals("Ground")) {
            // Update Animations
            animator.SetBool("Landed", true);
            animator.SetBool("Jump", false);
            // After Diving
            if (isDiving) {
                updatePlayerLocationBasedOnHealth(true); // Resets player position
            }
            // Update Player Status
            isJumping = false;
            isDiving = false;
        }
        // The Collision is an "Enemy"
        if (other.gameObject.CompareTag("Enemy")) {

            try
            {
                JumpSound[1].Play();
            }
            catch {
                Debug.Log("Audio not founded");
            }
            health = GameAssets.GetInstance().reducehealth();
            other.gameObject.transform.position=new Vector2(-100f, -100f);
            if (health <= 0) {
				body.bodyType = RigidbodyType2D.Static;
				if (OnDeath != null) 
					OnDeath(this, EventArgs.Empty);
                PlayerPrefs.SetInt("Score", GameAssets.GetInstance().getScore());
				SceneManager.LoadScene("GameOverMenu");
				PlayerPrefs.SetInt("Score", GameAssets.GetInstance().getScore());
            } else {
                updatePlayerLocationBasedOnHealth(true);
                isDiving = false;
                isJumping = false;
                isDucking = false;
                animator.SetBool("Jump", false);
                animator.SetBool("Ducking", false);
                animator.SetBool("Landed", true);
            }
        }
    }

    void handleJump() {
        body.velocity += Vector2.up * jumpVelocity;
        isJumping = true;
        if (!isDucking) { // Keep Animation from switching (rapidly)
            animator.SetBool("Jump", true);
            animator.SetBool("Ducking", false);
        }
        JumpSound[0].Play();
    }

    void handleDive() {
        body.velocity += Vector2.up * (jumpVelocity - 2f);
        // Rearrange colliders
        var size = boxCollider.size;
        boxCollider.size = new Vector2(size.x * 2, size.y / 2);
        var offset = circleCollider.offset;
        circleCollider.offset = new Vector2(offset.x, offset.y + 1.25f);
        isDiving = true;
        startDive = Time.realtimeSinceStartup;
        animator.SetBool("Landed", false);
        JumpSound[0].Play();
    }

    void handleDuck() {
        isDucking = true;
        boxCollider.size = new Vector2(boxCollider.size.x,boxCollider.size.y / 4);
        animator.SetBool("Ducking", true);
        if (isJumping) { // Keep Animation from switching (rapidly)
            animator.SetBool("Jump", false);
        }
    }

    void updatePlayerLocationBasedOnHealth(bool forceDefaultYPos) {
        float xPos = healthLocation + ((maxHealth - health) * fallBackHealth);
        float yPos = body.position.y;
        if (forceDefaultYPos) {
            yPos = -3.63f;
            if (isDiving) {
                var size = boxCollider.size;
                boxCollider.size = new Vector2(size.x / 2, size.y * 2);
                var offset = circleCollider.offset;
                circleCollider.offset = new Vector2(offset.x, offset.y - 1.25f);
            }
        }
        body.position = new Vector2(xPos,yPos);
    }
	
	public float GetPlayerPos() {
		return body.position.x;
	}
}
