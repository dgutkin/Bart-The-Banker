using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBehaviour : MonoBehaviour {

	private bool _isPaused = false;
	private Rect windowRect;

	// Use this for initialization
	void Start () {
		windowRect = new Rect (Screen.width / 2 - 300, Screen.height / 2 - 150, 600, 300);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {

		// attached to pause button game object

		_isPaused = !_isPaused;
		if (_isPaused) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

	}

	private void OnGUI() {
		if (_isPaused) {
			// need to create a GUI skin to change the background colour so its not transparent
			// need to cancel jump and slide if _isPaused
			windowRect = GUI.Window (0, windowRect, windowFunc, "Pause");
		}
	}

	private void windowFunc(int id) {
		if (GUILayout.Button ("Resume")) {
			_isPaused = false;
			Time.timeScale = 1;
		}
	}
}
