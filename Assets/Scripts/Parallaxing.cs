using UnityEngine;
using System.Collections;

public class Parallaxing : MonoBehaviour {

	public float parallaxScale;
	public float smoothing = 1f;

	private Transform cam;
	private Vector3 previousCamPosition;
	private Transform myTransform;

	// Called before Start()
	void Awake() {

		cam = Camera.main.transform;
		myTransform = transform;

	}

	// Use this for initialization
	void Start () {

		previousCamPosition = cam.position;
	
	}
	
	// Update is called once per frame
	void Update () {

		float parallax = (cam.position.x - previousCamPosition.x) * parallaxScale;
		float backgroundTargetPositionX = myTransform.position.x + parallax;
		Vector3 backgroundTargetPosition = new Vector3 (backgroundTargetPositionX, myTransform.position.y,
			                                   myTransform.position.z);
		myTransform.position = Vector3.Lerp (myTransform.position, backgroundTargetPosition, smoothing * Time.deltaTime);

		previousCamPosition = cam.position;
	
	}
}
