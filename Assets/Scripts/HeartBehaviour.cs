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

			AudioClip ting = GetComponent<AudioSource>().clip;
			AudioSource.PlayClipAtPoint(ting, transform.position);
			// use PlayClipAtPoint instead of Play because the game object is destroyed later
			Destroy (gameObject);

			other.SendMessage ("HitHeart", SendMessageOptions.DontRequireReceiver);
		}
	}
}
