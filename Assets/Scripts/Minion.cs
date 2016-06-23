using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Minion : MonoBehaviour 
{
	private int health = 3;
	public Transform targetPosition;
	private Rigidbody2D o_rigidbody;

	public List<Sprite> positionalSprites;

	void Awake()
	{
		o_rigidbody = GetComponent<Rigidbody2D>();
	}

	void Update () 
	{
		if (o_rigidbody.velocity.x > 0.5f && o_rigidbody.velocity.y > 0.5f)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[2];
		}
		else if (o_rigidbody.velocity.x > 0.5f && o_rigidbody.velocity.y < 0.5f)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[1];
		}
		else if (o_rigidbody.velocity.x > 0.5f && o_rigidbody.velocity.y <= 0.5f)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[6];
		}

		if (o_rigidbody.velocity.x < 0.5f && o_rigidbody.velocity.y > 0.5f)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[4];
		}
		else if (o_rigidbody.velocity.x < 0.5f && o_rigidbody.velocity.y < 0.5f)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[7];
		}
		else if (o_rigidbody.velocity.x < 0.5f && o_rigidbody.velocity.y <= 0.5f)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[5];
		}

		if (o_rigidbody.velocity.x <= 0.5f && o_rigidbody.velocity.y <= 0)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[0];
		}
		else if (o_rigidbody.velocity.x <= 0.5f && o_rigidbody.velocity.y > 0)
		{
			GetComponent<SpriteRenderer>().sprite = positionalSprites[3];
		}
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
			Destroy (gameObject);
		}
	}

	IEnumerator minionMovement()
	{
		while (health > 0)
		{
			yield return new WaitForSeconds(0.1f);


			Vector2 shootDirection = (Vector2)targetPosition.position - (Vector2)transform.position;
			shootDirection.Normalize();
			
			o_rigidbody.AddRelativeForce(shootDirection* 10);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("PlayerProjectile"))
		{
			damage();
		}
	}
}
