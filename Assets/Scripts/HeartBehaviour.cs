using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.gameObject.CompareTag ("Player")) {
			GetComponent<AudioSource> ().volume = PlayerPrefs.GetFloat (Constants.SOUND_VOLUME, Constants.DEFAULT_SOUND_VOLUME);

			AudioClip ting = GetComponent<AudioSource>().clip;
			AudioSource.PlayClipAtPoint(ting, transform.position);
			// use PlayClipAtPoint instead of Play because the game object is destroyed later
			Destroy (gameObject);

			other.SendMessage ("HitHeart", SendMessageOptions.DontRequireReceiver);
		}
	}
}
