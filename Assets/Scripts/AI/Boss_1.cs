using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss_1 : MonoBehaviour 
{
	// Stats
	private int maxHealth;
	public int health = 1000;
	public Text healthLabel;

	// Phasing
	private bool phasing;
	public int currentPhase;
	private bool phased;

	// Prefabs
	public GameObject boxPrefab;

	// Physics

	// Phase Variables
	private GameObject player1;
	private GameObject player2;
	private Rigidbody2D o_rigidbody;

	// Phase 0
	private GameObject tempBox = null;

	// Phase 1
	private Transform currentTarget;
	public float phase1MovementSpeed;
	public float phase1ShootSpeed;
	public GameObject phase1ProjectilePrefab;
	private int subphase = 1;

	// Phase 2
	public float phase2MovementSpeed;
	private bool landed;
	public GameObject minionPrefab;

	// Phase 3
	private Vector2 currentTargetPosition;
	private bool subphasing;
	public float phase3MovementSpeed;

	public Transform playerTransform;

	void Awake () 
	{
		player1 = GameObject.Find("Player_1").gameObject;
		player2 = GameObject.Find("Player_2").gameObject;

		// Initialize local variables
		o_rigidbody = GetComponent<Rigidbody2D>();

		maxHealth = health;
		healthLabel.text = health.ToString();
	}

	void Update () 
	{
		if (phased)
		{
			if ((health == ((maxHealth/3) * 2) || health == (maxHealth/3)))
			{
				phased = false;
				phasing = false;
				StopAllCoroutines();

				currentPhase++;
			}
		}
		else if (health != ((maxHealth/3) * 2) && health != (maxHealth/3))
		{
			phased = true;
		}

		movement();
	}

	void movement()
	{
		switch (currentPhase)
		{
			// Phase 0
			case 0:
				phase0();
				break;
			// Phase 1
			case 1:
				phase1();
				break;
			// Phase 2
			case 2:
				phase2();
				break;
			// Phase 3
			case 3:
				phase3();
				break;
			default:
				break;
		}
	}

	private void findClosestPlayer()
	{
		float player1Distance = Mathf.Abs((player1.transform.position - transform.position).magnitude);
		float player2Distance = Mathf.Abs((player2.transform.position - transform.position).magnitude);
		
		if (player1Distance < player2Distance)
		{
			currentTarget = player1.transform;
		}
		else
		{
			currentTarget = player2.transform;
		}
	}

	void phase0()
	{

	}

	void phase1()
	{
		findClosestPlayer();

		if (!phasing)
		{
			StartCoroutine(phase1MovementRoutines());
			StartCoroutine(phase1ShootingRoutines());
			transform.GetChild(1).GetComponent<BlendColors>().blendColors[0] = Color.red;

			phasing = true;
		}
	}

	void phase2()
	{
		if (!phasing)
		{
			StartCoroutine(phase2Routines());
			transform.GetChild(1).GetComponent<BlendColors>().blendColors[0] = Color.green;

			phasing = true;
		}
	}

	void phase3()
	{
		if (!phasing)
		{
			subphase = 1;
			StartCoroutine(phase3Routines());
			transform.GetChild(1).GetComponent<BlendColors>().blendColors[0] = Color.blue;

			phasing = true;
		}
	}

	IEnumerator phase0Routines()
	{
		while (currentPhase == 1)
		{
			yield return new WaitForSeconds(0.1f);

			// Movement
			o_rigidbody.AddRelativeForce((currentTarget.position - transform.position) * phase1MovementSpeed);
		}
	}

	IEnumerator phase1MovementRoutines()
	{
		while (currentPhase == 1)
		{
			yield return new WaitForSeconds(0.1f);
			
			// Movement
			o_rigidbody.AddRelativeForce((currentTarget.position - transform.position) * phase1MovementSpeed);
		}
	}

	IEnumerator phase1ShootingRoutines()
	{
		while (currentPhase == 1)
		{
			yield return new WaitForSeconds(0.5f);

			GameObject tempProjectile = (GameObject) Instantiate(phase1ProjectilePrefab, transform.position, Quaternion.identity);

			Vector2 shootDirection = player1.transform.position - transform.position;
			shootDirection.Normalize();

			tempProjectile.GetComponent<Rigidbody2D>().AddRelativeForce(shootDirection * phase1ShootSpeed);
		}
	}

	IEnumerator phase2Routines()
	{
		while (currentPhase == 2)
		{
			yield return new WaitForSeconds(0.1f);

			if (!landed)
			{
				// TODO: Move towards center of screen
				Vector2 enemyCenter = new Vector2(0.0f, 1.0f);

				// Movement
				o_rigidbody.AddRelativeForce((enemyCenter - (Vector2)transform.position) * phase2MovementSpeed);

				if (((Vector2)transform.position - enemyCenter).magnitude < 0.5f)
				{
					landed = true;
				}
			}
			else
			{
				yield return new WaitForSeconds(0.9f);

				findClosestPlayer();

				GameObject tempMinion = (GameObject) Instantiate(minionPrefab, transform.position, Quaternion.identity);
				tempMinion.GetComponent<Minion>().targetPosition = currentTarget;
				tempMinion.GetComponent<Minion>().startMinion();
			}
		}
	}

	// X: -6 to 6
	// Y: -3 to 3
	IEnumerator phase3Routines()
	{
		while (currentPhase == 3)
		{
			yield return new WaitForSeconds(0.1f);

			GameObject tempProjectile = (GameObject) Instantiate(phase1ProjectilePrefab, transform.position, Quaternion.identity);
			
			Vector2 shootDirection = player1.transform.position - transform.position;
			shootDirection.Normalize();
			
			tempProjectile.GetComponent<Rigidbody2D>().AddRelativeForce(shootDirection * phase1ShootSpeed);

			if (!subphasing)
			{
				if (subphase == 1)
				{
					// Movement
					currentTargetPosition = new Vector2(Random.Range(-6.0f, 6.0f), Random.Range(-3.0f, 3.0f));

					subphasing = true;
				}
			}
			else
			{
				// TODO: Check if destination is reached (or reached enough)
				Vector2 distanceVector = (Vector2)transform.position - currentTargetPosition;

				Debug.Log(distanceVector.magnitude.ToString());

				if (distanceVector.magnitude > 0.5f)
				{
					o_rigidbody.AddRelativeForce((currentTargetPosition - (Vector2)transform.position) * phase3MovementSpeed);
				}
				else
				{
					subphasing = false;
				}
			}
		}
	}

	IEnumerator startBoxPhase()
	{
		// Play pre-phase animation
		// TEMP: Simple rotational animation
		o_rigidbody.AddTorque(200.0f);

		// TODO: Replace time with how long animation will take to animate
		yield return new WaitForSeconds(3.0f);

		// Teleport players to center of screen
		playerTransform.position = Vector2.zero;

		// Spawn box in
		tempBox = (GameObject) Instantiate(boxPrefab, Vector3.zero, Quaternion.identity);

		// RANDOM: Move boss to random diagonal quadrant
		int randomQuadrant = Random.Range(1, 4);
		Vector2 newBossPosition = Vector2.zero;
		switch (randomQuadrant)
		{
			case 1:
				newBossPosition.x = -5;
				newBossPosition.y = 2.5f;
				break;
			case 2:
				newBossPosition.x = 5;
				newBossPosition.y = 2.5f;
				break;
			case 3:
				newBossPosition.x = -5;
				newBossPosition.y = -2.5f;
				break;
			case 4:
				newBossPosition.x = 5;
				newBossPosition.y = -2.5f;
				break;
		}
	}

	private void damage()
	{
		health--;
		healthLabel.text = health.ToString();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("PlayerProjectile"))
		{
			Destroy (other.gameObject);
			damage();
		}
	}
}
