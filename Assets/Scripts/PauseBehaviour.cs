using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseBehaviour : MonoBehaviour {

	public GameObject pauseScreen;
	public GameObject pauseText;
	public GameObject mainCamera;
	public GameObject quitButton;
	public GameObject quitText;
	public GameObject resumeButton;
	public GameObject resumeText;

	public delegate void PauseAction();
	public static event PauseAction OnPauseChange;

	private bool _isPaused = false;
	private Renderer _screenRenderer;
	private Text _textRenderer;
	private AudioSource _audioSource;
	private AudioSource _mainCameraAudioSource;
	private Renderer _quitButtonRenderer;
	private Text _quitTextRenderer;
	private Renderer _resumeButtonRenderer;
	private Text _resumeTextRenderer;

	// Use this for initialization
	void Start () {
		
		_screenRenderer = pauseScreen.GetComponent<Renderer>();
		_screenRenderer.enabled = false;
		_textRenderer = pauseText.GetComponent<Text> ();
		_textRenderer.enabled = false;
		_quitButtonRenderer = quitButton.GetComponent<Renderer> ();
		_quitButtonRenderer.enabled = false;
		_quitTextRenderer = quitText.GetComponent<Text> ();
		_quitTextRenderer.enabled = false;
		_resumeButtonRenderer = resumeButton.GetComponent<Renderer> ();
		_resumeButtonRenderer.enabled = false;
		_resumeTextRenderer = resumeText.GetComponent<Text> ();
		_resumeTextRenderer.enabled = false;
		_audioSource = GetComponent<AudioSource> ();
		_mainCameraAudioSource = mainCamera.GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity)) {

			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

			if ((hit.transform.CompareTag ("Resume")) &&
			    _resumeButtonRenderer.enabled) {

				Time.timeScale = 1f;
				StartCoroutine("ActivateResume");

			} else if (hit.transform.CompareTag ("Quit") && _quitButtonRenderer.enabled) {

				// Reset the time scale so that button activition runs
				// IEnumerator dependent on game time
				Time.timeScale = 1f;

			} else if (hit.transform.CompareTag ("Pause")) {

				_isPaused = !_isPaused;
				PauseGame ();

			}

		}

		// Update volumes
		_audioSource.volume = PlayerPrefs.GetFloat (Constants.SOUND_VOLUME, Constants.DEFAULT_SOUND_VOLUME);
		_mainCameraAudioSource.volume = PlayerPrefs.GetFloat (Constants.SOUND_VOLUME, Constants.DEFAULT_SOUND_VOLUME);
		
	}

	void PauseGame() {

		// Play button click sound
		_audioSource.Play ();

		_screenRenderer.enabled = true;
		_textRenderer.enabled = true;
		_quitButtonRenderer.enabled = true;
		_quitTextRenderer.enabled = true;
		_resumeButtonRenderer.enabled = true;
		_resumeTextRenderer.enabled = true;
		Time.timeScale = 0;
		_mainCameraAudioSource.Pause();

		OnPauseChange ();

	}

	void UnPauseGame() {

		// Play button click sound
		_audioSource.Play ();

		_screenRenderer.enabled = false;
		_textRenderer.enabled = false;
		_quitButtonRenderer.enabled = false;
		_quitTextRenderer.enabled = false;
		_resumeButtonRenderer.enabled = false;
		_resumeTextRenderer.enabled = false;
		//Time.timeScale = 1;
		_mainCameraAudioSource.Play();

		OnPauseChange ();

	}

	IEnumerator ActivateResume() {

		_resumeButtonRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1f);

		_audioSource.Play();

		// Only the first part of the clip is the required sound
		yield return new WaitForSeconds(_audioSource.clip.length * 0.3f);

		// set the sprite back to normal
		_resumeButtonRenderer.material.color = new Color(1f, 1f, 1f, 1f);

		_isPaused = !_isPaused;
		UnPauseGame ();

	}

}
