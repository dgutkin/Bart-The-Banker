using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public Transform player;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		this.gameObject.transform.position = new Vector3(player.position.x, this.gameObject.transform.position.y,this.gameObject.transform.position.z);

	}
}
