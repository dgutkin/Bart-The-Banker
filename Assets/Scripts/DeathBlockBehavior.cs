using UnityEngine;
using System.Collections;

public class DeathBlockBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Kill player on collision
	void OnTriggerEnter2D(Collider2D other) {

		if (other.gameObject.CompareTag ("Player")) {
			other.SendMessage ("HitObstacle", SendMessageOptions.DontRequireReceiver);
		}

	}
}
