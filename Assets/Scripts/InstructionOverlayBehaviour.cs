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
			// Only show the overlay with instructions if less than 3 high scores recorded
			List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));
			if (leaderboards.Count < 3) {
				_showOverlay = true;
				_rightSide.enabled = true;
				_leftSide.enabled = true;
			}

		}

	}
	
	// Update is called once per frame
	void Update () {

		if (_showOverlay) {

			float showDuration = 3f;
			float fadeDuration = 0.5f;
			StartCoroutine(ShowOverlay(_rightSide, _leftSide, fadeDuration, showDuration));
			_showOverlay = false;

		}
		
	}

	public IEnumerator ShowOverlay(Text rightText, Text leftText, float fadeDuration, float showDuration) {

		yield return new WaitForSeconds (showDuration);
		rightText.CrossFadeAlpha (0f, fadeDuration, false);
		leftText.CrossFadeAlpha (0f, fadeDuration, false);
		yield return new WaitForSeconds (fadeDuration);
		_rightSide.enabled = false;
		_leftSide.enabled = false;

	}

}
