using UnityEngine;
using System.Collections;

public class DeathBlockBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//  Player loses a life on collision
	void OnTriggerEnter2D(Collider2D other) {

		if (other.gameObject.CompareTag ("Player")) {
			
			Destroy (gameObject);
			other.SendMessage ("HitObstacle", SendMessageOptions.DontRequireReceiver);

		}

	}

}