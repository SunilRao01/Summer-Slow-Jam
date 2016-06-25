﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TEMP Directional Sprite Movement


public class Player : MonoBehaviour
{
	enum Direction {none, left, up, right, down};

	public bool godMode;
	public float health;
	private float maxHealth;

	// Audio clips
	public AudioClip soundShoot, soundCombinedShoot;
	public AudioClip soundStep, soundCombinedStep;
	public AudioClip soundRegen;
	public AudioClip soundCombine, soundSeparate;

	// TEMP No Animation
	// Note: Down, Up, Left, Right
	private SpriteRenderer o_spriteRenderer;
	public List<Sprite> facingDirectionSprites; 
	private GameManager o_gameManager;

	// Movement
	public float playerMovementSpeed;
	public float playerMaxMovementSpeed;
	private Rigidbody2D o_rigidbody;
	private float horizontalMovement;
	private float verticalMovement;

	// Shooting
	// Individual Quad Directional
	public GameObject playerProjectilePrefab;
	public float playerShootRate;
	public float bulletSpeed;
	private bool isShootingHorizontal = false;
	private bool isShootingVertical = false;
	private bool isQuadShootRunning = false;
	private Direction quadShootDirection;

	// Health Regeneration
	public float regenInterval, regenWait;
	private float regenIntervalTimer, regenWaitTimer;

	// Combined Full Rotational Shooting
	public bool combined;
	public GameObject playerCombinedProjectilePrefab;
	public float playerCombinedShootRate;
	public float combinedBulletSpeed;
	private bool isShooting = false;
	private Vector2 rotationalShootDirection;
	public List<Sprite> combinedFacingDirectionSprites;
	private float horizontalShooting;
	private float verticalShooting;
	
	// Animations and effects
	private float flashTime;
	private List<Sprite> currentDirectionalSprites;

	// References to object components
	private GameObject o_otherPlayerObject;
	private Player o_otherPlayer;
	private AudioSource o_audioSource;
	private GenericSprite o_genericSprite;
	private BoxCollider2D o_collider;

	void Awake()
	{
		// Initialize local variables
		o_spriteRenderer = GetComponent<SpriteRenderer> ();
		o_rigidbody      = GetComponent<Rigidbody2D> ();
		o_audioSource    = GetComponent<AudioSource> ();
		o_genericSprite  = GetComponent<GenericSprite> ();
		o_collider		 = GetComponent<BoxCollider2D> ();
		o_gameManager    = GameObject.Find("GameManager").GetComponent<GameManager>();
		maxHealth = health;

		// Maintain a reference to our other player.
		if (gameObject.name == "Player_1")
			o_otherPlayerObject = GameObject.Find("Player_2");
		else
			o_otherPlayerObject = GameObject.Find("Player_1");
		o_otherPlayer = o_otherPlayerObject.GetComponent<Player> ();

		// Default direction currently facing up
		o_spriteRenderer.sprite = facingDirectionSprites[0];
		currentDirectionalSprites = facingDirectionSprites;

		Physics2D.IgnoreCollision (o_collider,
			o_otherPlayerObject.GetComponent<Collider2D>());
	}

	void Start () 
	{
	
	}

	void Update()
	{
		// TEMP For dev
		devModeCommands();

		if (gameObject.name == "Player_2")
		{
			if (Input.GetButtonDown("Seperate") || Input.GetKeyDown (KeyCode.Space))
			{
				float dist = Vector3.Distance (transform.position,
				   o_otherPlayerObject.transform.position);
				if (dist < 0.50f || combined)
					toggleCombinedMode();
			}
			if (combined)
				rotationShooting();
			else
				quadShooting ();
		}
		else
		{
			if (!combined)
				quadShooting();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		// Regenerate Health
		regenerateHealth (Time.deltaTime);
	}

	void regenerateHealth (float t)
	{
		// Don't start regenerating health until a certain time after damage has passed.
		if (regenWaitTimer > 0.00f) {
			regenWaitTimer -= t;
			if (regenWaitTimer <= 0.00f) {
				regenWaitTimer = 0.00f;
				regenIntervalTimer = 0.00f;
			}
    		else
				return;
		}
		regenIntervalTimer -= Time.deltaTime;
		while (regenIntervalTimer <= 0.00f) {
			regenIntervalTimer += regenInterval;
			if (health < maxHealth) {
				health = Mathf.Min (health + 1.00f, maxHealth);
				if (soundRegen)
					o_audioSource.PlayOneShot (soundRegen);
				updateColor ();
				o_genericSprite.AddGlow (new Vector4 (-0.50f, 1.00f, -0.50f));
			}
		}
		if (health >= maxHealth)
			return;
	}
	
	void FixedUpdate () 
	{
		if (gameObject.name == "Player_1")
		{
			horizontalMovement = Input.GetAxis("Horizontal");
			verticalMovement = Input.GetAxis("Vertical");
			
			horizontalShooting = Input.GetAxis("ShootHorizontal");
			verticalShooting = Input.GetAxis("ShootVertical");
		}
		else if (gameObject.name == "Player_2")
		{
			horizontalMovement = Input.GetAxis("Horizontal_2");
			verticalMovement = Input.GetAxis("Vertical_2");
			
			horizontalShooting = Input.GetAxis("ShootHorizontal_2");
			verticalShooting = Input.GetAxis("ShootVertical_2");
		}

		if (combined)
		{
			if (gameObject.name == "Player_1")
			{
				combinedMovement();
			}
		}
		else
		{
			movement();
		}
	}

	void devModeCommands()
	{
		if (Input.GetButtonDown("DEV Mode Switch"))
		{
			Debug.Log("Switch mode");
			toggleCombinedMode();
		}
	}

	void toggleCombinedMode()
	{
		if (gameObject.name == "Player_2") {
			o_otherPlayer.toggleCombinedModeReal ();
			toggleCombinedModeReal ();
		} else {
			toggleCombinedModeReal ();
			o_otherPlayer.toggleCombinedModeReal ();
		}
		if (combined && soundCombine)
			o_audioSource.PlayOneShot (soundCombine);
		if (!combined && soundSeparate)
			o_audioSource.PlayOneShot (soundSeparate);
	}
	
	public void toggleCombinedModeReal ()
	{
		isShooting = false;
		isShootingHorizontal = false;
		isShootingVertical = false;

		// Stop current shooting
		StopAllCoroutines();

		if (combined)
		{
			combined = false;
			// If we've the first one to separate, update player positions.
			if (o_otherPlayer.combined) {
				Vector3 pos = transform.position;
				transform.position               = pos + new Vector3 (-0.33f, 0.00f, 0.00f);
				o_otherPlayer.transform.position = pos + new Vector3 ( 0.33f, 0.00f, 0.00f);
			}
			if (gameObject.name == "Player_2") {
				GetComponent<BoxCollider2D>().enabled = true;
				GetComponent<SpriteRenderer>().enabled = true;
			}
			else {
				playerMovementSpeed = 150;
				transform.localScale /= 1.33f;
			}
		}
		else
		{
			combined = true;
			if (gameObject.name == "Player_2")
			{
				GetComponent<BoxCollider2D>().enabled = false;
				GetComponent<SpriteRenderer>().enabled = false;
				transform.position = o_otherPlayer.transform.position;
			}
			else if (gameObject.name == "Player_1")
			{
				playerMovementSpeed = 35;
				transform.localScale *= 1.33f;
			}
		}

		// Change sprite
		if (!combined)
		{
			currentDirectionalSprites = facingDirectionSprites;
		}
		else
		{
			currentDirectionalSprites = combinedFacingDirectionSprites;
		}
		o_spriteRenderer.sprite = currentDirectionalSprites [0];
	}

	void combinedMovement()
	{
		if (gameObject.name == "Player_1")
		{
			o_otherPlayer.transform.position = transform.position;

			Vector2 movementDirection = new Vector2(horizontalMovement, verticalMovement);
			movementDirection *= playerMovementSpeed;
			
			// TEMP Set sprite by direction
			// Note: Down, Up, Left, Right
			if (horizontalMovement == 1)
			{
				o_spriteRenderer.sprite = currentDirectionalSprites[3];
			}
			else if (horizontalMovement == -1)
			{
				o_spriteRenderer.sprite = currentDirectionalSprites[2];
			}
			if (verticalMovement == 1)
			{
				o_spriteRenderer.sprite = currentDirectionalSprites[1];
			}
			else if (verticalMovement == -1)
			{
				o_spriteRenderer.sprite = currentDirectionalSprites[0];
			}
			
			if (o_rigidbody.velocity.magnitude < playerMaxMovementSpeed)
			{
				o_rigidbody.AddForce(movementDirection);
			}
		}
	}

	void movement()
	{
		Vector2 movementDirection = new Vector2(horizontalMovement, verticalMovement);

		// Temp keyboard controls
		if (Input.GetKey(KeyCode.A))
		{
			movementDirection.x -= 1;
		}

		 if (Input.GetKey(KeyCode.S))
		{
			movementDirection.y -= 1;
		}

		 if (Input.GetKey(KeyCode.D))
		{
			movementDirection.x += 1;
		}


		if (Input.GetKey(KeyCode.W))
		{
			movementDirection.y += 1;
		}


		movementDirection *= playerMovementSpeed;



		// TEMP Set sprite by direction
		// Note: Down, Up, Left, Right
		if (horizontalMovement == 1 || Input.GetKey(KeyCode.D))
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[3];
		}
		else if (horizontalMovement == -1 || Input.GetKey(KeyCode.A))
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[2];
		}
		if (verticalMovement == 1 || Input.GetKey(KeyCode.W))
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[1];
		}
		else if (verticalMovement == -1 || Input.GetKey(KeyCode.S))
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[0];
		}

		if (o_rigidbody.velocity.magnitude < playerMaxMovementSpeed)
		{
			o_rigidbody.AddForce(movementDirection.normalized * 100);
		}
	}

	void quadShooting()
	{
		// Get our axis values.
		float axisH = horizontalShooting,
		      axisV = verticalShooting;

		// Which axis has the highest priority?
		Direction dir = Direction.none;
		if (Mathf.Abs (axisH) >= Mathf.Abs (axisV)) {
			if (axisH < -0.50f)
				dir = Direction.left;
			else if (axisH > 0.50f)
				dir = Direction.right;
		}
		else {
			if (axisV < -0.50f)
				dir = Direction.down;
			else if (axisV > 0.50f)
				dir = Direction.up;
		}

		// Let keyboard controls override the joystick.
		if (Input.GetKey (KeyCode.LeftArrow))
			dir = Direction.left;
		else if (Input.GetKey (KeyCode.RightArrow))
			dir = Direction.right;
		else if (Input.GetKey (KeyCode.DownArrow))
			dir = Direction.down;
		else if (Input.GetKey (KeyCode.UpArrow))
			dir = Direction.up;

		// Store our shooting state.
		quadShootDirection = dir;
		switch (dir) {
			case Direction.left:
			case Direction.right:
				isShootingHorizontal = true;
				isShootingVertical = false;
				break;

			case Direction.up:
			case Direction.down:
				isShootingHorizontal = false;
				isShootingVertical = true;
				break;

			default:
				isShootingHorizontal = false;
				isShootingVertical = false;
				break;
		}

		// Do we need to start shooting?
		if (dir != Direction.none && !isQuadShootRunning) {
			isQuadShootRunning = true;
			StartCoroutine (quadShoot ());
		}
	}

	IEnumerator quadShoot()
	{
		while (isShootingHorizontal || isShootingVertical)
		{
			GameObject tempProjectile = (GameObject) Instantiate(playerProjectilePrefab, transform.position, Quaternion.identity);
			if (soundShoot)
				o_audioSource.PlayOneShot (soundShoot);

			Vector2 projectileDirection = Vector2.zero;
			switch (quadShootDirection)
			{
				case Direction.left:
				{
					tempProjectile.transform.Rotate(Vector3.forward, 90);
					projectileDirection.x = -1;
					break;
				}
				case Direction.up:
				{
					projectileDirection.y = 1;
					break;
				}
				case Direction.right:
				{
					tempProjectile.transform.Rotate(Vector3.forward,270);	
					projectileDirection.x = 1;
					break;
				}
				case Direction.down:
				{
					tempProjectile.transform.Rotate(Vector3.forward, 180);	
					projectileDirection.y = -1;
					break;
				}
			}
			projectileDirection *= bulletSpeed;

			tempProjectile.GetComponent<Rigidbody2D>().AddForce(projectileDirection);
			yield return new WaitForSeconds(playerShootRate);			
		}
		isQuadShootRunning = false;
	}
		
	void rotationShooting()
	{
		//Debug.Log("Axis: " + horizontalShooting + ", " + verticalShooting);
		if (horizontalShooting != 0 || verticalShooting != 0)
		{
			rotationalShootDirection = new Vector2 (horizontalShooting, verticalShooting).normalized * 4.00f;
			if (!isShooting)
			{
				isShooting = true;
				StartCoroutine(rotShoot());
			}
		}
		else
		{
			rotationalShootDirection = Vector2.up;
			isShooting = false;
			StopAllCoroutines();
		}
	}

	IEnumerator rotShoot()
	{
		while (isShooting)
		{
			yield return new WaitForSeconds(playerCombinedShootRate);

			GameObject tempProjectile = (GameObject) Instantiate(playerCombinedProjectilePrefab, transform.position, Quaternion.identity);
			if (soundCombinedShoot)
				o_audioSource.PlayOneShot (soundCombinedShoot);			

			Vector2 projectileDirection = rotationalShootDirection;
			projectileDirection *= combinedBulletSpeed;
			
			tempProjectile.GetComponent<Rigidbody2D>().AddForce(projectileDirection);
			float angle = Mathf.Atan2 (projectileDirection.y, projectileDirection.x);
			tempProjectile.transform.Rotate (Vector3.forward, angle / Mathf.PI * 180.00f - 90.00f);
		}
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		// LOSE
		if (!godMode)
		{
			bool destroy = false, hurt = false;
			if (other.gameObject.CompareTag("Minion")) {
			    hurt = true;
			}
			else if (other.gameObject.CompareTag("EnemyProjectile")) {
				destroy = true;
				hurt = true;
			}
			if (destroy) {
				Destroy (other.gameObject);
			}
			if (hurt) {
				// Hurt ourselves.  If we didn't end the game (and we're combined), hurt the other player as well.
				if (!damage () && combined)
					o_otherPlayer.damage ();
			}
		}
	}

	bool damage()
	{
		health -= 1.00f;
		regenWaitTimer = regenWait;
		if (health <= 0.00f) {
			o_gameManager.win = false;
			Application.LoadLevel (2);
			return true;
		}
		else {
			updateColor ();
			GetComponent<GenericSprite>().damage ();
			return false;
		}
	}

	void updateColor ()
	{
		float percent = health / maxHealth;
		Vector4 newColor = new Vector4 (
			1.00f + percent / 2.00f,
			percent * 1.50f - 0.50f,
			percent * 1.50f - 0.50f,
			1.00f);
		GetComponent<Renderer>().material.SetVector ("_Color", newColor);
	}
}
