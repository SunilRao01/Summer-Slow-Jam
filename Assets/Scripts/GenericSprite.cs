using UnityEngine;
using System.Collections;

public class GenericSprite : MonoBehaviour
{
	public float SortOffset;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	public void Update () {
		GetComponent<SpriteRenderer>().sortingOrder = (int)((transform.position.y + SortOffset) * -100.00);
	}
}
