using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour {

	public float parallaxScale;
	public float smoothing = 1f;

	private Transform _cam;
	private Vector3 _previousCamPosition;
	private Transform _myTransform;

	// Called before Start()
	void Awake() {

		_cam = Camera.main.transform;
		_myTransform = transform;

	}

	// Use this for initialization
	void Start () {

		_previousCamPosition = _cam.position;
	
	}
	
	// Update is called once per frame
	void Update () {

		float parallax = (_cam.position.x - _previousCamPosition.x) * parallaxScale;
		float backgroundTargetPositionX = _myTransform.position.x + parallax;
		Vector3 backgroundTargetPosition = new Vector3 (
			backgroundTargetPositionX, 
			_myTransform.position.y,
			_myTransform.position.z
		);
		_myTransform.position = Vector3.Lerp (
			_myTransform.position, 
			backgroundTargetPosition, 
			smoothing * Time.deltaTime
		);

		_previousCamPosition = _cam.position;
	
	}
}
