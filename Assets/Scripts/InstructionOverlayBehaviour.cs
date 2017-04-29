using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionOverlayBehaviour : MonoBehaviour {

	public GameObject rightSideInstructions;
	public GameObject leftSideInstructions;

	private bool _showOverlay = false;
	private Text _rightSide;
	private Text _leftSide;

	// Use this for initialization
	void Start () {
		
		_rightSide = rightSideInstructions.GetComponent<Text> ();
		_rightSide.enabled = false;
		_leftSide = leftSideInstructions.GetComponent<Text> ();
		_leftSide.enabled = false;

		if (PlayerPrefs.HasKey ("leaderboards")) {
			// Only show the overlay with instructions if less than 4 high scores recorded
			List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));
			if (leaderboards.Count < 4) {
				_showOverlay = true;
				_rightSide.enabled = true;
				_leftSide.enabled = true;
			}
		} else {
			_showOverlay = true;
			_rightSide.enabled = true;
			_leftSide.enabled = true;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (_showOverlay) {

			float showDuration = 3f;
			float fadeDuration = 0.5f;
			StartCoroutine(Utility.FadeTextOut(_rightSide, showDuration, fadeDuration));
			StartCoroutine(Utility.FadeTextOut(_leftSide, showDuration, fadeDuration));
			_showOverlay = false;

		}
		
	}

}
