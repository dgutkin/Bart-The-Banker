using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public Transform player;
	public int distanceFromCenter = 3;
	public Transform background;

	private Transform _cameraTransform;

	// Use this for initialization
	void Start () {
		
		_cameraTransform = GetComponent<Transform> ();

	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		_cameraTransform.position = new Vector3(player.position.x + distanceFromCenter, 
			_cameraTransform.position.y,
			_cameraTransform.position.z);

	}
}
