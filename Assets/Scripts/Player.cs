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
	//private bool shooting;

	void Awake()
	{
		// Initialize local variables
		rigidbody = GetComponent<Rigidbody2D>();
	}

	void Start () 
	{
	
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
		if (Input.GetButtonDown("HorizontalShooting"))
		{
			StartCoroutine(shoot(Direction.left));
		}
	}

	IEnumerator shoot(Direction direction)
	{
		while (true)
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
