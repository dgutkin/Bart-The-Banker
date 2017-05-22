using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionOverlayBehaviour : MonoBehaviour {

	public GameObject rightSideInstructions;
	public GameObject leftSideInstructions;
	public GameObject avoidInstructions;
	public GameObject avoidCage;
	public GameObject avoidCop;
	public GameObject avoidTax;

	private bool _showOverlay = false;
	private Text _rightSide;
	private Text _leftSide;
	private Text _avoidText;
	private SpriteRenderer _avoidCageRenderer;
	private SpriteRenderer _avoidCopRenderer;
	private SpriteRenderer _avoidTaxRenderer;

	// Use this for initialization
	void Start () {
		
		_rightSide = rightSideInstructions.GetComponent<Text> ();
		_rightSide.enabled = false;
		_leftSide = leftSideInstructions.GetComponent<Text> ();
		_leftSide.enabled = false;
		_avoidText = avoidInstructions.GetComponent<Text> ();
		_avoidText.enabled = false;
		_avoidCageRenderer = avoidCage.GetComponent<SpriteRenderer> ();
		_avoidCageRenderer.enabled = false;
		_avoidCopRenderer = avoidCop.GetComponent<SpriteRenderer> ();
		_avoidCageRenderer.enabled = false;
		_avoidTaxRenderer = avoidTax.GetComponent<SpriteRenderer> ();
		_avoidTaxRenderer.enabled = false;

		if (PlayerPrefs.HasKey ("leaderboards")) {
			// Only show the overlay with instructions if less than 4 high scores recorded
			List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));
			if (leaderboards.Count < 400) {
				_showOverlay = true;
				_rightSide.enabled = true;
				_leftSide.enabled = true;
				_avoidText.enabled = true;
				_avoidCageRenderer.enabled = true;
				_avoidCopRenderer.enabled = true;
				_avoidTaxRenderer.enabled = true;
			}
		} else {
			_showOverlay = true;
			_rightSide.enabled = true;
			_leftSide.enabled = true;
			_avoidText.enabled = true;
			_avoidCageRenderer.enabled = true;
			_avoidCopRenderer.enabled = true;
			_avoidTaxRenderer.enabled = true;
		}

	}
	
	// Update is called once per frame
	void Update () {

		if (_showOverlay) {

			float showDuration = 3f;
			float fadeDuration = 0.5f;
			StartCoroutine(Utility.FadeTextOut(_rightSide, showDuration, fadeDuration));
			StartCoroutine(Utility.FadeTextOut(_leftSide, showDuration, fadeDuration));
			StartCoroutine (Utility.FadeTextOut (_avoidText, showDuration, fadeDuration));
			StartCoroutine (Utility.FadeSpriteOut (_avoidCageRenderer, showDuration, fadeDuration));
			StartCoroutine (Utility.FadeSpriteOut (_avoidCopRenderer, showDuration, fadeDuration));
			StartCoroutine (Utility.FadeSpriteOut (_avoidTaxRenderer, showDuration, fadeDuration));
			_showOverlay = false;

		}
		
	}

}
