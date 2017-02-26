using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public Transform player;
	public int distanceFromCenter = 3;
	public float cameraSpeed = 2.8f;
	public Transform background;

	private Transform _cameraTransform;
	private float positionDifference;

	// Use this for initialization
	void Start () {
		
		_cameraTransform = GetComponent<Transform> ();


	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		//this.gameObject.transform.position = new Vector3(player.position.x + distanceFromCenter, 
		//	this.gameObject.transform.position.y,
		//	this.gameObject.transform.position.z);

		this.gameObject.transform.Translate (cameraSpeed * Time.deltaTime, 0, 0);
		// make an adjustment to slow down or speed up the camera if its position
		// deviates too far from the position of player
		positionDifference = player.position.x - _cameraTransform.position.x;
		// express positionDifference as a percent of the screen width and adjust
		// the camera speed by the change
		float adjustment = positionDifference / Screen.width;
		cameraSpeed = cameraSpeed * (1 + adjustment);
		// looks a little choppy...need more smoothing

	}
}
