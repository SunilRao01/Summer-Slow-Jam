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
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		sr.sortingOrder = (int)
			((transform.position.y +
			  sr.sprite.bounds.min.y +
			  SortOffset) * -100.00);
	}
}
