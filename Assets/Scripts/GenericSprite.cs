using UnityEngine;
using System.Collections;

public class GenericSprite : MonoBehaviour
{
	// Public settings, including audio.
	public float SortOffset;
	public AudioClip audioDamage;

	// Internal settings.
	private float timeInterval = 0.10f;
	private Vector4 glow;

	// References to other components.
	private SpriteRenderer o_srenderer;
	private Renderer o_renderer;
	private AudioSource o_audio;

	// Use this for initialization
	void Start () {
		o_srenderer = GetComponent<SpriteRenderer> ();
		o_renderer  = GetComponent<Renderer> ();
		o_audio		= GetComponent<AudioSource> ();
		StartCoroutine (updateSprite ());
	}
	
	// Update is called once per frame
	IEnumerator updateSprite () {
		while (true) {
			o_srenderer.sortingOrder = (int)
				((transform.position.y +
				o_srenderer.sprite.bounds.min.y +
				SortOffset) * -100.00);
			if (glow.magnitude >= 0.00f) {
				glow -= glow * timeInterval * 5.00f;
				if (glow.magnitude < 0.00f)
					glow = Vector4.zero;
				o_renderer.material.SetVector ("_Glow", glow);
			}
			yield return new WaitForSeconds (timeInterval);
		}
	}

	public void damage () {
		AddGlow (new Vector4 (1.00f, 1.00f, 1.00f, 0.00f));
		if (audioDamage)
			o_audio.PlayOneShot (audioDamage);
	}

	public void AddGlow (Vector4 color) {
		glow += color;
	}
}
