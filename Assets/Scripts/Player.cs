using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TEMP Directional Sprite Movement


public class Player : MonoBehaviour 
{
	enum Direction {left, up, right, down};

	public float health;

	// TEMP No Animation
	// Note: Down, Up, Left, Right
	private SpriteRenderer o_spriteRenderer;
	public List<Sprite> facingDirectionSprites; 

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

	void Awake()
	{
		// Initialize local variables
		o_spriteRenderer = GetComponent<SpriteRenderer>();
		o_rigidbody = GetComponent<Rigidbody2D>();

		// Default direction currently facing up
		o_spriteRenderer.sprite = facingDirectionSprites[1];
		currentDirectionalSprites = facingDirectionSprites;

	}

	void Start () 
	{
	
	}

	void Update()
	{
		// TEMP For dev
		devModeCommands();

		if (combined)
		{
			rotationShooting();
		}
		else
		{
			quadShooting();
		}
	}
	
	void FixedUpdate () 
	{
		if (combined)
		{
			combinedMovement();
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
		}
		else
		{
			combined = true;
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
		Vector2 movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		movementDirection *= playerMovementSpeed;
		
		// TEMP Set sprite by direction
		// Note: Down, Up, Left, Right
		if (Input.GetAxis("Horizontal") == 1)
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[3];
		}
		else if (Input.GetAxis("Horizontal") == -1)
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[2];
		}
		if (Input.GetAxis("Vertical") == 1)
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[1];
		}
		else if (Input.GetAxis("Vertical") == -1)
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[0];
		}
		
		if (o_rigidbody.velocity.magnitude < playerMaxMovementSpeed)
		{
			o_rigidbody.AddForce(movementDirection);
		}
	}

	void movement()
	{
		Vector2 movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		movementDirection *= playerMovementSpeed;

		// TEMP Set sprite by direction
		// Note: Down, Up, Left, Right
		if (Input.GetAxis("Horizontal") == 1)
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[3];
		}
		else if (Input.GetAxis("Horizontal") == -1)
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[2];
		}
		if (Input.GetAxis("Vertical") == 1)
		{
			o_spriteRenderer.sprite = currentDirectionalSprites[1];
		}
		else if (Input.GetAxis("Vertical") == -1)
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
		if (!isShootingHorizontal)
		{
			if (Input.GetAxis("ShootHorizontal") == -1)
			{
				isShootingVertical = false;
				StopAllCoroutines();

				quadShootDirection = Direction.left;
				isShootingHorizontal = true;
				StartCoroutine(quadShoot(quadShootDirection));
			}
			else if (Input.GetAxis("ShootHorizontal") == 1)
			{
				isShootingVertical = false;
				StopAllCoroutines();

				quadShootDirection = Direction.right;
				isShootingHorizontal = true;
				StartCoroutine(quadShoot(quadShootDirection));
			}
		}
		if (!isShootingVertical)
		{
			if (Input.GetAxis("ShootVertical") == -1)
			{
				isShootingHorizontal = false;
				StopAllCoroutines();

				quadShootDirection = Direction.down;
				isShootingVertical = true;
				StartCoroutine(quadShoot(quadShootDirection));
			}
			else if (Input.GetAxis("ShootVertical") == 1)
			{
				isShootingHorizontal = false;
				StopAllCoroutines();

				quadShootDirection = Direction.up;
				isShootingVertical = true;
				StartCoroutine(quadShoot(quadShootDirection));
			}
		}

		if (isShootingHorizontal)
		{
			if (Input.GetAxis("ShootHorizontal") != 1 && Input.GetAxis("ShootHorizontal") != -1)
			{
				isShootingHorizontal = false;
				StopCoroutine(quadShoot(Direction.left));
				StopCoroutine(quadShoot(Direction.right));
			}
		}
		if (isShootingVertical)
		{
			if (Input.GetAxis("ShootVertical") != 1 && Input.GetAxis("ShootVertical") != -1)
			{
				isShootingVertical = false;
				StopCoroutine(quadShoot(Direction.up));
				StopCoroutine(quadShoot(Direction.down));
			}
		}

	}

	IEnumerator quadShoot(Direction direction)
	{
		while (isShootingHorizontal || isShootingVertical)
		{
			yield return new WaitForSeconds(playerShootRate);

			GameObject tempProjectile = (GameObject) Instantiate(playerProjectilePrefab, transform.position, Quaternion.identity);

			Vector2 projectileDirection = Vector2.zero;
			switch (direction)
			{
				case Direction.left:
				{
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
					projectileDirection.x = 1;
					break;
				}
				case Direction.down:
				{
					projectileDirection.y = -1;
					break;
				}
			}
			projectileDirection *= bulletSpeed;

			tempProjectile.GetComponent<Rigidbody2D>().AddForce(projectileDirection);
		}
	}

	void rotationShooting()
	{
		//Debug.Log("Axis: " + Input.GetAxis("ShootHorizontal") + ", " + Input.GetAxis("ShootVertical"));
		if (Input.GetAxis("ShootHorizontal") != 0 || Input.GetAxis("ShootVertical") != 0)
		{
			Vector2 lookPosition = transform.position;
			lookPosition.x += (Input.GetAxis("ShootHorizontal") * 5);
			lookPosition.y += (Input.GetAxis("ShootVertical") * 5);
			
			rotationalShootDirection = (lookPosition - (Vector2)transform.position);
			
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
		}
	}
}
