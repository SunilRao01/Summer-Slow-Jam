using UnityEngine;
using System.Collections;

public class GenericSprite : MonoBehaviour
{
	public float SortOffset;
	private float GlowTime;
	public float timeInterval = 0.10f;
	private SpriteRenderer o_srenderer;
	private Renderer o_renderer;

	// Use this for initialization
	void Start () {
		o_srenderer = GetComponent<SpriteRenderer> ();
		o_renderer  = GetComponent<Renderer> ();
		StartCoroutine (updateSprite ());
	}
	
	// Update is called once per frame
	IEnumerator updateSprite () {
		while (true) {
			o_srenderer.sortingOrder = (int)
				((transform.position.y +
				o_srenderer.sprite.bounds.min.y +
				SortOffset) * -100.00);
			if (GlowTime >= 0.00f) {
				GlowTime -= timeInterval;
				if (GlowTime <= 0.00f) {
					GlowTime = 0.00f;
					o_renderer.material.SetFloat ("_Glow", 0.00f);
				} else {
					o_renderer.material.SetFloat ("_Glow", GlowTime);
				}
			}
			yield return new WaitForSeconds (timeInterval);
		}
	}

	public void damage () {
		GlowTime = 0.50f;
	}
}
