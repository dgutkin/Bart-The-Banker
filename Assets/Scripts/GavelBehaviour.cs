using UnityEngine;
using System.Collections;

public class GavelBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other) {

		if (other.gameObject.CompareTag ("Player")) {
			other.SendMessage ("hitGavel", SendMessageOptions.DontRequireReceiver);
		}

	}

}
