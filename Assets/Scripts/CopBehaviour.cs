using UnityEngine;
using System.Collections;

public class CopBehaviour : MonoBehaviour {

	public float walkingSpeed = 2.3f;
	public float leftTurnAroundDelay = 0f;

	private float _leftTurnAroundTime;
	private bool _walkingLeft;
	private float _distanceWalked;
	private float _leftBound;
	private float _rightBound;
	private Animator _copAnimator;

	// Use this for initialization
	void Start () {
		
		_distanceWalked = 0f;
		_walkingLeft = true;
		_leftBound = transform.position.x - 1.0f;
		_rightBound = transform.position.x + 1.0f;
		_copAnimator = GetComponent<Animator> ();
		UpdateWalkOrientation ();

	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > _leftTurnAroundTime) { // start walking right after delay
			
			// Just walk in one way, rotation dictates direction
			transform.Translate (Vector3.left * walkingSpeed * Time.deltaTime);

			if (_copAnimator.speed == 0) {
				_copAnimator.speed = 1; // resume animation
			}

		}
		// Swap directions once past bounds
		if (transform.position.x >= _rightBound || transform.position.x <= _leftBound) {
			
			// Snap position back to just behind before swapping directions
			float newX = transform.position.x >= _rightBound ? _rightBound - 0.001f : _leftBound + 0.001f;
			transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
			SwitchDirections ();

		}

	}

	public void SwitchDirections() {

		_walkingLeft = !_walkingLeft;
		UpdateWalkOrientation ();

	}

	void UpdateWalkOrientation() {
		
		if (_walkingLeft) {
			
			transform.localRotation = Quaternion.Euler (0, 180, 0);
			_leftTurnAroundTime = Time.time + leftTurnAroundDelay;
			_copAnimator.speed = 0; // stop the animation

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