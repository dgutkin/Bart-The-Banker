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

	public PlayerController playerController;

	private bool _showOverlay = false;
	private Text _rightSide;
	private Text _leftSide;
	private Text _avoidText;
	private SpriteRenderer _avoidCageRenderer;
	private SpriteRenderer _avoidCopRenderer;
	private SpriteRenderer _avoidTaxRenderer;

	private int _instructionIndex;
	private Animator _playerAnimator;

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
		_avoidCopRenderer.enabled = false;
		_avoidTaxRenderer = avoidTax.GetComponent<SpriteRenderer> ();
		_avoidTaxRenderer.enabled = false;

		_playerAnimator = playerController.GetComponent<Animator>();

		if (PlayerPrefs.HasKey ("leaderboards") && PlayerPrefs.HasKey ("showinstructionoverlay")) {
			List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));
			if (leaderboards.Count < 100 && PlayerPrefs.GetInt ("showinstructionoverlay") == 1) {
				playerController.moveSpeed = 1f;
				_playerAnimator.speed = 1f / 3f;
				_instructionIndex = 1;
			} else {
				_instructionIndex = 0;
			}
		} else {
			playerController.moveSpeed = 1f;
			_instructionIndex = 1;
		}

	}
	
	// Update is called once per frame
	void Update () {

		float showDuration = 3f;
		float fadeDuration = 0.5f;

		// only go to the next instruction once all texts are disabled
		if (_rightSide.enabled == false && _leftSide.enabled == false && _avoidText.enabled == false
			&& _instructionIndex < 5) {

			switch (_instructionIndex) {
			case 0:
				// do nothing
				break;
			case 1:
				// tap the right side to jump
				_rightSide.enabled = true;
				StartCoroutine(Utility.FadeTextOut(_rightSide, showDuration, fadeDuration));
				break;
			case 2:
				// hold the left side to slide
				_leftSide.enabled = true;
				StartCoroutine(Utility.FadeTextOut(_leftSide, showDuration, fadeDuration));
				break;
			case 3:
				// avoid the cages, taxes and cops
				_avoidText.enabled = true;
				_avoidCageRenderer.enabled = true;
				_avoidTaxRenderer.enabled = true;
				_avoidCopRenderer.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_avoidText, showDuration, fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_avoidCageRenderer, showDuration, fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_avoidTaxRenderer, showDuration, fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_avoidCopRenderer, showDuration, fadeDuration));
				break;
			case 4:
				playerController.moveSpeed = 3f;
				_playerAnimator.speed = 1f;
				break;
			}

			_instructionIndex++;

		}
	}



}
