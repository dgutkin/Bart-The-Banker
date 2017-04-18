using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class VolumeSliderBehaviour : MonoBehaviour {

	public Slider slider;
	public string volumeName;

	private string volumeKey;
	private float defaultVolume;

	// Use this for initialization
	void Start () {
		slider = GetComponent<Slider> ();

		switch (volumeName) {
		case "Music":
			volumeKey = Constants.MUSIC_VOLUME;
			defaultVolume = Constants.DEFAULT_MUSIC_VOLUME;
			break;
		case "Sound":
			volumeKey = Constants.SOUND_VOLUME;
			defaultVolume = Constants.DEFAULT_SOUND_VOLUME;
			break;
		default:
			defaultVolume = 0.3f;
			break;
		}

		slider.value = PlayerPrefs.GetFloat (volumeKey, defaultVolume);
	}
	
	// Update is called once per frame
	void Update () {
		if (volumeName == "Sound") {
			AudioListener.volume = slider.value;
		}

		PlayerPrefs.SetFloat (volumeKey , slider.value);
	}
}

