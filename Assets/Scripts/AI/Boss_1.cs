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
			// Phase 1
			case 1:
				phase1 ();
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

	void phase1()
	{
		// TODO: Find closes player
		findClosestPlayer();

		if (!phasing)
		{
			StartCoroutine(phase1Routines());
			phasing = true;
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
