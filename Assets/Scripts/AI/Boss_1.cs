﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Boss_1 : MonoBehaviour 
{
	// Stats
	private GameManager gameManager;
	private int maxHealth;
	public int health = 1000;
	public Text healthLabel;
	public AudioClip soundShoot, soundThink;

	// Phasing
	private bool phasing;
	public int currentPhase;
	private const int maxPhases = 4;
	private int phaseDamage;
	private bool phased;

	// Prefabs
	public GameObject boxPrefab;

	// Physics

	// Phase Variables
	private GameObject player1;
	private GameObject player2;
	private Rigidbody2D o_rigidbody;
	private GenericSprite o_genericSprite;
	private AudioSource o_audioSource;

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
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		player1 = GameObject.Find("Player_1").gameObject;
		player2 = GameObject.Find("Player_2").gameObject;
		o_genericSprite = GetComponent<GenericSprite> ();
		o_audioSource = GetComponent<AudioSource> ();

		// Initialize local variables
		o_rigidbody = GetComponent<Rigidbody2D>();
		
		maxHealth = health;
		healthLabel.text = health.ToString();
	}

	void Update () 
	{
		// WIN
		if (health <= 0)
		{
			gameManager.win = true;
			Application.LoadLevel(2);
		}

		// Have we taken enough damage to start the next phase?
		if (currentPhase < maxPhases) {
			while (phaseDamage >= (maxHealth / 3)) {
				phased = false;
				phasing = false;
				StopAllCoroutines ();
				currentPhase++;
				phaseDamage -= (maxHealth / 3);
				thinkSound ();
			}
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
		float player1Distance = Vector4.Distance (player1.transform.position, transform.position);
		float player2Distance = Vector4.Distance (player2.transform.position, transform.position);

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
		if (!phased)
		{
			StartCoroutine(initialPause());
			phased = true;
		}
	}

	void thinkSound ()
	{
		if (soundThink)
			o_audioSource.PlayOneShot (soundThink);
	}

	IEnumerator initialPause()
	{
		yield return new WaitForSeconds(3.0f);
		phased = false;
		currentPhase++;
		thinkSound ();
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
			if (soundShoot)
				o_audioSource.PlayOneShot (soundShoot);
			o_genericSprite.AddGlow (new Vector4 (0.50f, 0.00f, 0.00f, 0.00f));

			Vector2 shootDirection = currentTarget.position - transform.position;
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

				GameObject[] minions = GameObject.FindGameObjectsWithTag ("Minion");
				if (minions.Length < 15) {
					float angle = Random.Range (0.00f, Mathf.PI * 2.00f);
					Vector3 direction = new Vector3 (
						Mathf.Cos (angle), Mathf.Sin (angle), 0.00f);
					GameObject tempMinion = (GameObject) Instantiate(minionPrefab, transform.position + direction * 0.75f, Quaternion.identity);
					tempMinion.GetComponent<Rigidbody2D>().velocity = direction * 4.00f;
					tempMinion.GetComponent<Minion>().targetPosition = currentTarget;
					tempMinion.GetComponent<Minion>().startMinion();
					tempMinion.tag = "Minion";
					o_genericSprite.AddGlow (new Vector4 (-0.50f, 0.50f, -0.50f, 0.00f));
				}
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
			if (soundShoot)
				o_audioSource.PlayOneShot (soundShoot);
			
			Vector2 shootDirection = currentTarget.position - transform.position;
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
		phaseDamage++;
		health--;
		healthLabel.text = health.ToString();
		GetComponent<GenericSprite>().damage ();
		transform.Find ("Boss_1_Glow").GetComponent<GenericSprite> ().damage ();
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
