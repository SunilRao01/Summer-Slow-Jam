using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{
	enum Direction {left, up, right, down};

	// Movement
	public float playerMovementSpeed;
	public float playerMaxMovementSpeed;
	private Rigidbody2D rigidbody;

	// Shooting
	public GameObject playerProjectilePrefab;
	public float playerShootRate;
	public float bulletSpeed;
	private bool isShooting = false;

	void Awake()
	{
		// Initialize local variables
		rigidbody = GetComponent<Rigidbody2D>();
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

		if (rigidbody.velocity.magnitude < playerMaxMovementSpeed)
		{
			rigidbody.AddForce(movementDirection);
		}
	}

	void shooting()
	{
		if (Input.GetButtonDown("FireLeft"))
		{
			isShooting = true;
			StartCoroutine(shoot(Direction.left));
		}
		else if (Input.GetButtonDown("FireRight"))
		{
			isShooting = true;
			StartCoroutine(shoot(Direction.right));
		}
		else if (Input.GetButtonDown("FireDown"))
		{
			isShooting = true;
			StartCoroutine(shoot(Direction.down));
		}
		else if (Input.GetButtonDown("FireUp"))
		{
			isShooting = true;
			StartCoroutine(shoot(Direction.up));
		}

		if (Input.GetButtonUp("FireLeft"))
		{
			isShooting = false;
			StopCoroutine(shoot(Direction.left));
		}
		if (Input.GetButtonUp("FireRight"))
		{
			isShooting = false;
			StopCoroutine(shoot(Direction.right));
		}
		if (Input.GetButtonUp("FireDown"))
		{
			isShooting = false;
			StopCoroutine(shoot(Direction.down));
		}
		if (Input.GetButtonUp("FireUp"))
		{
			isShooting = false;
			StopCoroutine(shoot(Direction.up));
		}
	}

	IEnumerator shoot(Direction direction)
	{
		while (isShooting)
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
