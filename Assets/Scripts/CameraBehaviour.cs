using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public Transform player;
	public float distanceFromCenter = 3.0f;

	private Transform _cameraTransform;

	// Use this for initialization
	void Start () {
		
		_cameraTransform = GetComponent<Transform> ();
		GetComponent<AudioSource> ().volume = PlayerPrefs.GetFloat (Constants.MUSIC_VOLUME, Constants.DEFAULT_MUSIC_VOLUME);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		_cameraTransform.position = new Vector3(
			player.position.x + distanceFromCenter, 
			_cameraTransform.position.y,
			_cameraTransform.position.z
		);

	}
}
