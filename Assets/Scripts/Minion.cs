using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minion : MonoBehaviour 
{
	private int health = 3;
	public Transform targetPosition;
	public GameObject myShadow;
	public float maxVelocity;
	public List<Sprite> positionalSprites;

    // Audio clips
	public AudioClip soundSpawn, soundDie;

	// References to components
	private Rigidbody2D    o_rigidbody;
	private SpriteRenderer o_renderer;
	private AudioSource    o_audioSource;

	void Awake()
	{
		o_rigidbody   = GetComponent<Rigidbody2D>();
		o_renderer    = GetComponent<SpriteRenderer> ();
		o_audioSource = GetComponent<AudioSource> ();
		if (soundSpawn)
			o_audioSource.PlayOneShot (soundSpawn);
	}

	void Start ()
	{
		// Add some randomness to our velocity so minions don't clump together.
		maxVelocity *= Random.Range (0.75f, 1.25f);

		// Spawn a shadow.
		GameObject shadow = Instantiate (myShadow, 
		   transform.position + new Vector3 (0.00f, -0.25f, 0.00f),
		   transform.rotation) as GameObject;
		shadow.transform.parent = transform;
	}

	void Update () 
	{
	}

	public void startMinion()
	{
		if (targetPosition != null)
		{
			StartCoroutine(minionMovement());
		}
	}

	void damage()
	{
		health--;

		if (health < 0)
		{
			if (soundDie)
				o_audioSource.PlayOneShot (soundDie);
			Destroy (gameObject);
		}
		GetComponent<GenericSprite>().damage ();
	}

	IEnumerator minionMovement()
	{
		while (health > 0)
		{
			yield return new WaitForSeconds(0.1f);

			Vector2 shootDirection = (Vector2)targetPosition.position - (Vector2)transform.position;

			// Apply movement.
			shootDirection.Normalize();
			o_rigidbody.velocity = o_rigidbody.velocity + shootDirection * 0.5f;

			// What direction are we facing?  Get the angle from our velocity.
			int dir = ((int) Mathf.Round (
				Mathf.Atan2 (o_rigidbody.velocity.y, o_rigidbody.velocity.x) /
				Mathf.PI * 4.00f + 8.00f)) % 8;
			o_renderer.sprite = positionalSprites [dir];

			// Don't go too fast.
			if (o_rigidbody.velocity.magnitude > maxVelocity)
				o_rigidbody.velocity = o_rigidbody.velocity.normalized * maxVelocity;
		}
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
