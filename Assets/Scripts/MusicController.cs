using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

	private AudioSource _audioSource;

	// Use this for initialization
	void Start () {
		
	}

	void Awake() {
		
		_audioSource = GetComponent<AudioSource> ();
		_audioSource.ignoreListenerVolume = true;
		_audioSource.volume = PlayerPrefs.GetFloat (Constants.MUSIC_VOLUME, Constants.DEFAULT_MUSIC_VOLUME);
		_audioSource.Play ();

	}
	
	// Update is called once per frame
	void Update () {

		_audioSource.volume = PlayerPrefs.GetFloat (Constants.MUSIC_VOLUME, Constants.DEFAULT_MUSIC_VOLUME);

	}
}
