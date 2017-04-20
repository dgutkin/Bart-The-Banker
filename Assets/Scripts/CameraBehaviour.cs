using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public Transform player;

	private AudioSource _audioSource;
	private Transform _cameraTransform;

	// Use this for initialization
	void Start () {
		
		_cameraTransform = GetComponent<Transform> ();

	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		_cameraTransform.position = new Vector3(
			player.position.x + Constants.PLAYER_DISTANCE_FROM_CENTER, 
			_cameraTransform.position.y,
			_cameraTransform.position.z
		);

	}

	void Update() {
		_audioSource.volume = PlayerPrefs.GetFloat (Constants.MUSIC_VOLUME, Constants.DEFAULT_MUSIC_VOLUME);
	}

	void Awake() {
		_audioSource = GetComponent<AudioSource> ();
		_audioSource.ignoreListenerVolume = true;
		_audioSource.volume = PlayerPrefs.GetFloat (Constants.MUSIC_VOLUME, Constants.DEFAULT_MUSIC_VOLUME);
		_audioSource.Play ();
	}
}
