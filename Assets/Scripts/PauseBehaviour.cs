using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseBehaviour : MonoBehaviour {

	public GameObject pauseScreen;
	public GameObject pauseText;
	public GameObject mainCamera;

	public delegate void PauseAction();
	public static event PauseAction OnPauseChange;

	private bool _isPaused = false;
	private Renderer _screenRenderer;
	private Text _textRenderer;
	private AudioSource _audioSource;
	private AudioSource _mainCameraAudioSource;

	// Use this for initialization
	void Start () {
		
		_screenRenderer = pauseScreen.GetComponent<Renderer>();
		_screenRenderer.enabled = false;
		_textRenderer = pauseText.GetComponent<Text> ();
		_textRenderer.enabled = false;
		_audioSource = GetComponent<AudioSource> ();
		_mainCameraAudioSource = mainCamera.GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Detect if pause screen is tapped and unpause
		if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity)) {

			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

			if (hit.transform.CompareTag("Pause") && _screenRenderer.enabled) {
				
				_isPaused = !_isPaused;
				UnPauseGame ();

			}

		}

		// Update volumes
		_audioSource.volume = PlayerPrefs.GetFloat (Constants.SOUND_VOLUME, Constants.DEFAULT_SOUND_VOLUME);
		_mainCameraAudioSource.volume = PlayerPrefs.GetFloat (Constants.SOUND_VOLUME, Constants.DEFAULT_SOUND_VOLUME);
		
	}

	void OnMouseDown() {

		// attached to pause button game object
		_isPaused = !_isPaused;

		if (_isPaused) {
			
			PauseGame ();

		} else {
			
			UnPauseGame ();

		}

	}

	void PauseGame() {

		// Play button click sound
		_audioSource.Play ();

		_screenRenderer.enabled = true;
		_textRenderer.enabled = true;
		Time.timeScale = 0;
		_mainCameraAudioSource.Pause();

		OnPauseChange ();

	}

	void UnPauseGame() {

		// Play button click sound
		_audioSource.Play ();

		Time.timeScale = 1;
		_screenRenderer.enabled = false;
		_textRenderer.enabled = false;
		_mainCameraAudioSource.Play();

		OnPauseChange ();

	}

}
