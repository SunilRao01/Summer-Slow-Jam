using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TEMP Directional Sprite Movement


public class Player : MonoBehaviour
{
	enum Direction {none, left, up, right, down};

	public bool godMode;

	public float health;

	// TEMP No Animation
	// Note: Down, Up, Left, Right
	private SpriteRenderer o_spriteRenderer;
	public List<Sprite> facingDirectionSprites; 
	private GameManager gameManager;

	// Movement
	public float playerMovementSpeed;
	public float playerMaxMovementSpeed;
	private Rigidbody2D o_rigidbody;

	// Shooting
	// Individual Quad Directional
	public GameObject playerProjectilePrefab;
	public float playerShootRate;
	public float bulletSpeed;
	private bool isShootingHorizontal = false;
	private bool isShootingVertical = false;
	private bool isQuadShootRunning = false;
	private Direction quadShootDirection;

	// Combined Full Rotational Shooting
	private bool combined;
	public GameObject playerCombinedProjectilePrefab;
	public float playerCombinedShootRate;
	public float combinedBulletSpeed;
	private bool isShooting = false;
	private Vector2 rotationalShootDirection;
	public List<Sprite> combinedFacingDirectionSprites;
	
	private List<Sprite> currentDirectionalSprites;

	// Animations and effects
	private float flashTime;

	private GameObject o_otherPlayer;
	private float horizontalMovement;
	private float verticalMovement;
	private float horizontalShooting;
	private float verticalShooting;

	void Awake()
	{


		// Initialize local variables
		o_spriteRenderer = GetComponent<SpriteRenderer>();
		o_rigidbody = GetComponent<Rigidbody2D>();

		if (gameObject.name == "Player_1")
		{
			o_otherPlayer = GameObject.Find("Player_2");
		}
		else
		{
			o_otherPlayer = GameObject.Find("Player_1");
		}
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

		// Default direction currently facing up
		o_spriteRenderer.sprite = facingDirectionSprites[0];
		currentDirectionalSprites = facingDirectionSprites;

	}

	void Start () 
	{
	
	}

	void Update()
	{
		// TEMP For dev
		devModeCommands();

		if (combined && gameObject.name == "Player_2")
		{
			rotationShooting();

			if (Input.GetButtonDown("Seperate"))
			{
				toggleCombinedMode();
			}
		}
		else if (!combined)
		{
			quadShooting();
		}
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
		isShooting = false;
		isShootingHorizontal = false;
		isShootingVertical = false;

		// Stop current shooting
		StopAllCoroutines();



		if (combined)
		{
			combined = false;

			playerMovementSpeed = 150;

			if (gameObject.name == "Player_2")
			{
				GetComponent<BoxCollider2D>().enabled = true;
				GetComponent<Rigidbody2D>().isKinematic = false;
				GetComponent<SpriteRenderer>().enabled = true;
			}
		}
		else
		{
			combined = true;


			playerMovementSpeed -= 20;

			if (gameObject.name == "Player_2")
			{
				GetComponent<BoxCollider2D>().enabled = false;
				GetComponent<Rigidbody2D>().isKinematic = true;
				GetComponent<SpriteRenderer>().enabled = false;

				transform.position = o_otherPlayer.transform.position;
			}
			else if (gameObject.name == "Player_1")
			{
				playerMovementSpeed = 75;
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
			o_rigidbody.AddForce(movementDirection);
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

			// TODO: rotate projectile sprite relative to shoot direction

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
			Vector2 lookPosition = transform.position;
			lookPosition.x += (horizontalShooting * 5);
			lookPosition.y += (verticalShooting * 5);
			
			rotationalShootDirection = (lookPosition - (Vector2)transform.position);
			rotationalShootDirection.Normalize();

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

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			// Change shooting mode
			toggleCombinedMode();
		}


	}

	IEnumerator rotShoot()
	{
		while (isShooting)
		{
			yield return new WaitForSeconds(playerCombinedShootRate);

			GameObject tempProjectile = (GameObject) Instantiate(playerCombinedProjectilePrefab, transform.position, Quaternion.identity);
			
			Vector2 projectileDirection = rotationalShootDirection;
			projectileDirection *= combinedBulletSpeed;
			
			tempProjectile.GetComponent<Rigidbody2D>().AddForce(projectileDirection);

			if (projectileDirection.x > 0)
			{
				tempProjectile.transform.Rotate(Vector3.forward, 270);
			}
			else if (projectileDirection.x < 0)
			{
				tempProjectile.transform.Rotate(Vector3.forward, 90);
			}
			else if (projectileDirection.y < 0)
			{
				tempProjectile.transform.Rotate(Vector3.forward, 180);
			}
		}
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		// LOSE
		if (!godMode)
		{
			if (other.gameObject.CompareTag("Minion") 
			    || other.gameObject.CompareTag("EnemyProjectile"))
			{
				gameManager.win = false;
				Application.LoadLevel(2);
			}
		}
	}

}
