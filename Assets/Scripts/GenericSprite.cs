using UnityEngine;
using System.Collections;

public class GenericSprite : MonoBehaviour
{
	public float SortOffset;
	private float GlowTime;

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
		if (GlowTime >= 0.00f) {
			Renderer r = GetComponent<Renderer> ();
			GlowTime -= Time.deltaTime;
			if (GlowTime <= 0.00f) {
				GlowTime = 0.00f;
				r.material.SetFloat ("_Glow", 0.00f);
			}
			else {
				r.material.SetFloat ("_Glow", GlowTime);
			}
		}
	}

	public void damage () {
		GlowTime = 0.50f;
	}
}
