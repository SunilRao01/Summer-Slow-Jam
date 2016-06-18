using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	enum Direction {left, up, right, down};

	public float health;

	// Movement
	public float playerMovementSpeed;
	public float playerMaxMovementSpeed;
	private Rigidbody2D body;

	// Shooting
	public GameObject playerProjectilePrefab;
	public float playerShootRate;
	public float bulletSpeed;
	private bool isShooting = false;
	private Direction shootDirection;

	void Awake()
	{
		// Initialize local variables
		body = GetComponent<Rigidbody2D>();
	}

	void Start () 
	{
	
	}

	void Update()
	{
		shooting();
	}
	
	void FixedUpdate () 
	{
		movement();
	}

	void movement()
	{
		Vector2 movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		movementDirection *= playerMovementSpeed;

		if (body.velocity.magnitude < playerMaxMovementSpeed)
		{
			body.AddForce(movementDirection);
		}
	}

	void shooting()
	{

		if (Input.GetButtonDown("FireLeft"))
		{
			if (isShooting)
			{
				isShooting = false;
				StopCoroutine(shoot(shootDirection));
			}

			shootDirection = Direction.left;
			isShooting = true;
			StartCoroutine(shoot(shootDirection));
		}
		else if (Input.GetButtonDown("FireRight"))
		{
			if (isShooting)
			{
				Debug.Log("Stopping shooting in " + shootDirection + " direction");
				isShooting = false;
				StopCoroutine(shoot(shootDirection));
			}

			shootDirection = Direction.right;
			isShooting = true;
			StartCoroutine(shoot(shootDirection));
		}
		else if (Input.GetButtonDown("FireDown"))
		{
			if (isShooting)
			{
				isShooting = false;
				StopCoroutine(shoot(shootDirection));
			}

			shootDirection = Direction.down;
			isShooting = true;
			StartCoroutine(shoot(shootDirection));
		}
		else if (Input.GetButtonDown("FireUp"))
		{
			if (isShooting)
			{
				isShooting = false;
				StopCoroutine(shoot(shootDirection));
			}

			shootDirection = Direction.up;
			isShooting = true;
			StartCoroutine(shoot(shootDirection));
		}

		if (Input.GetButtonUp("FireLeft"))
		{
			if (shootDirection == Direction.left)
			{
				isShooting = false;
			}
			StopCoroutine(shoot(Direction.left));
		}
		if (Input.GetButtonUp("FireRight"))
		{
			if (shootDirection == Direction.right)
			{
				isShooting = false;
			}
			StopCoroutine(shoot(Direction.right));
		}
		if (Input.GetButtonUp("FireDown"))
		{
			if (shootDirection == Direction.down)
			{
				isShooting = false;
			}
			StopCoroutine(shoot(Direction.down));
		}
		if (Input.GetButtonUp("FireUp"))
		{
			if (shootDirection == Direction.up)
			{
				isShooting = false;
			}
			StopCoroutine(shoot(Direction.up));
		}

	}

	IEnumerator shoot(Direction direction)
	{
		while (isShooting && shootDirection == direction)
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
}
