using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour 
{
	void Awake()
	{
		StartCoroutine(selfDestruct());
	}

	IEnumerator selfDestruct()
	{
		yield return new WaitForSeconds(3);

		Destroy(gameObject);
	}
}
