using UnityEngine;
using System.Collections;

public class CopBehaviour : MonoBehaviour {

	public float walkingSpeed  = 0.5f;
	public float turnAroundTime = 5.0f;
	private bool _walkingLeft = true;
	private float _distanceWalked;

	// Use this for initialization
	void Start () {
	
		_distanceWalked = 0f;
		updateWalkOrientation ();

	}
	
	// Update is called once per frame
	void Update () {
		
		if (_distanceWalked >= turnAroundTime) {
			_distanceWalked = 0f;
			switchDirections ();
		}

		if (!_walkingLeft) {
			transform.Translate (new Vector3 (walkingSpeed * Time.deltaTime, 0.0f, 0.0f));
		} else {
			transform.Translate (new Vector3 (walkingSpeed * Time.deltaTime * -1.0f, 0.0f, 0.0f));
		}

		_distanceWalked += Time.deltaTime;



	}

	public void switchDirections() {

		_walkingLeft = !_walkingLeft;
		updateWalkOrientation ();

	}

	void updateWalkOrientation() {
		
		Vector3 localScale = transform.localScale;
		Debug.Log (localScale.ToString ());
		if (_walkingLeft) {
			if (localScale.x < 0.0f) {
				localScale.x = localScale.x * -1.0f;
				transform.localScale = localScale;
			}
		} else {
			if (localScale.x > 0.0f) {
				localScale.x = localScale.x * -1.0f;
				transform.localScale = localScale;
			}
		}

	}

	void OnTriggerEnter2D(Collider2D otherObj) {
		if (otherObj.gameObject.CompareTag("Player")) {
			otherObj.SendMessage ("HitObstacle", SendMessageOptions.DontRequireReceiver);
		}
	}


}
