using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseBehaviour : MonoBehaviour {

	public GameObject pauseScreen;
	public GameObject pauseText;

	public delegate void PauseAction();
	public static event PauseAction OnPauseChange;

	private bool _isPaused = false;
	private Renderer _screenRenderer;
	private Text _textRenderer;

	// Use this for initialization
	void Start () {
		
		_screenRenderer = pauseScreen.GetComponent<Renderer>();
		_screenRenderer.enabled = false;
		_textRenderer = pauseText.GetComponent<Text> ();
		_textRenderer.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Detect if pause screen is tapped and unpause
		if (Input.GetMouseButtonDown(0) && Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity)) {

			RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

			if (hit.transform.CompareTag("Pause")) {
				
				_isPaused = !_isPaused;
				Time.timeScale = 1;
				_screenRenderer.enabled = false;
				_textRenderer.enabled = false;
				AudioListener.pause = false;
				OnPauseChange ();

			}

		}
		
	}

	void OnMouseDown() {

		// attached to pause button game object
		_isPaused = !_isPaused;
		if (_isPaused) {
			
			_screenRenderer.enabled = true;
			_textRenderer.enabled = true;
			Time.timeScale = 0;
			AudioListener.pause = true;

			OnPauseChange ();

		} else {
			
			Time.timeScale = 1;
			_screenRenderer.enabled = false;
			_textRenderer.enabled = false;
			AudioListener.pause = false;
			OnPauseChange ();

		}

	}



}
