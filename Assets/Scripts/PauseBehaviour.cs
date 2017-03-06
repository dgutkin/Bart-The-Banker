using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PauseBehaviour : MonoBehaviour {

	private bool _isPaused = false;
	private Rect windowRect;
	private Renderer screenRenderer;
	private Text textRenderer;

	public GameObject pauseScreen;
	public GameObject pauseText;

	public delegate void PauseAction();
	public static event PauseAction OnPauseChange;

	// Use this for initialization
	void Start () {
		screenRenderer = pauseScreen.GetComponent<Renderer>();
		screenRenderer.enabled = false;
		textRenderer = pauseText.GetComponent<Text> ();
		textRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {

		// attached to pause button game object

		_isPaused = !_isPaused;
		if (_isPaused) {
			screenRenderer.enabled = true;
			textRenderer.enabled = true;
			Time.timeScale = 0;
			OnPauseChange ();
		} else {
			Time.timeScale = 1;
			screenRenderer.enabled = false;
			textRenderer.enabled = false;
			OnPauseChange ();
		}

	}

}
