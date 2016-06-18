using UnityEngine;
using System.Collections;

public class Boss_1 : MonoBehaviour 
{
	// Phasing
	public int currentPhase;

	// Prefabs
	public GameObject boxPrefab;

	// Physics
	private Rigidbody2D body;

	// Phase Variables
	private GameObject tempBox = null;

	public Transform playerTransform;

	void Awake () 
	{
		// Initialize local variables
		body = GetComponent<Rigidbody2D>();
	}

	void Start()
	{

	}

	void Update () 
	{

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
