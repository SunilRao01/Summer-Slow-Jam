using UnityEngine;
using System.Collections;

public class Boss_1 : MonoBehaviour 
{
	// Phasing
	private bool phasing;
	public int currentPhase;

	// Prefabs
	public GameObject boxPrefab;

	// Physics
	private Rigidbody2D body;

	// Phase Variables
	private GameObject player1;
	private GameObject player2;
	private Rigidbody2D o_rigidbody;

	// Phase 0
	private GameObject tempBox = null;

	// Phase 1
	private Transform currentTarget;
	public float phase1MovementSpeed;

	// Phase 2
	public float phase2MovementSpeed;


	public Transform playerTransform;

	void Awake () 
	{
		player1 = GameObject.Find("Player_1").gameObject;
		player2 = GameObject.Find("Player_2").gameObject;

		// Initialize local variables
		o_rigidbody = GetComponent<Rigidbody2D>();
	}

	void Start()
	{

	}

	void Update () 
	{
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
			StartCoroutine(phase1Routines());
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
	IEnumerator phase1Routines()
	{
		while (currentPhase == 1)
		{
			yield return new WaitForSeconds(0.1f);
			
			// Movement
			o_rigidbody.AddRelativeForce((currentTarget.position - transform.position) * phase1MovementSpeed);
		}
	}
	IEnumerator phase2Routines()
	{
		while (currentPhase == 2)
		{
			yield return new WaitForSeconds(0.1f);

			// TODO: Move towards center of screen
			Vector2 enemyCenter = new Vector2(0.0f, 1.0f);

			// Movement
			o_rigidbody.AddRelativeForce((enemyCenter - (Vector2)transform.position) * phase2MovementSpeed);
		}
	}
	IEnumerator phase3Routines()
	{
		while (currentPhase == 3)
		{
			yield return new WaitForSeconds(0.1f);

			// Movement
			o_rigidbody.AddRelativeForce((currentTarget.position - transform.position) * phase1MovementSpeed);
		}
	}

	IEnumerator startBoxPhase()
	{
		// Play pre-phase animation
		// TEMP: Simple rotational animation
		body.AddTorque(200.0f);

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
}
