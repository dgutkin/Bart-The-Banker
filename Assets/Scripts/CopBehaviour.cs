using UnityEngine;
using System.Collections;

public class CopBehaviour : MonoBehaviour {

	public float walkingSpeed  = 0.0005f;
	private bool _walkingLeft;
	private float _distanceWalked;
	private float _leftBound;
	private float _rightBound;

	// Use this for initialization
	void Start () {
		
		_distanceWalked = 0f;
		_walkingLeft = true;
		_leftBound = transform.position.x - 1.0f;
		_rightBound = transform.position.x + 1.0f;
		updateWalkOrientation ();

	}
	
	// Update is called once per frame
	void Update () {

		// Just walk in one direction since when flip direction, the direction will change accordingly
		transform.Translate (Vector3.left * walkingSpeed * Time.deltaTime);

		// Swap directions once past bounds
		if (transform.position.x >= _rightBound || transform.position.x <= _leftBound) {
			// Snap position back to just below before swapping directions
			float newX = transform.position.x >= _rightBound ? _rightBound - 0.001f : _leftBound + 0.001f;
			transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
			switchDirections ();
		}
	}

	public void switchDirections() {

		_walkingLeft = !_walkingLeft;
		updateWalkOrientation ();

	}

	void updateWalkOrientation() {
		
		if (_walkingLeft) {
			transform.localRotation = Quaternion.Euler (0, 180, 0);
		} else {
			transform.localRotation = Quaternion.Euler (0, 0, 0);
		}

	}

	void OnTriggerEnter2D(Collider2D otherObj) {
		if (otherObj.gameObject.CompareTag("Player")) {
			otherObj.SendMessage ("HitObstacle", SendMessageOptions.DontRequireReceiver);
		}
	}


}
