using UnityEngine;
using System.Collections;

public class Minion : MonoBehaviour 
{
	private int health = 5;
	public Vector3 targetPosition;
	private Rigidbody2D o_rigidbody;

	void Start () 
	{

		StartCoroutine(minionMovement());
	}
	
	void Update () 
	{
		
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


			Vector2 shootDirection = targetPosition - transform.position;
			shootDirection.Normalize();
			
			o_rigidbody.AddRelativeForce(shootDirection);
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
