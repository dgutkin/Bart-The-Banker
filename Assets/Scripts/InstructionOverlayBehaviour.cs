using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionOverlayBehaviour : MonoBehaviour {

	public GameObject rightSideInstructions;
	public GameObject rightSideBoundary;
	public GameObject leftSideInstructions;
	public GameObject leftSideBoundary;
	public GameObject avoidInstructions;
	public GameObject avoidCage;
	public GameObject avoidCop;
	public GameObject avoidTax;
	public GameObject bribeCop;
	public GameObject bribeCopInstructions;
	public GameObject platformJump;
	public GameObject platformJumpInstructions;

	public PlayerController playerController;

	private bool _showOverlay = false;
	private Text _rightSide;
	private Text _leftSide;
	private LineRenderer _rightSideBoundaryRenderer;
	private LineRenderer _leftSideBoundaryRenderer;

	private Text _avoidText;
	private SpriteRenderer _avoidCageRenderer;
	private SpriteRenderer _avoidCopRenderer;
	private SpriteRenderer _avoidTaxRenderer;

	private SpriteRenderer _bribeCopRenderer;
	private SpriteRenderer _platformJumpRenderer;
	private Text _bribeCopText;
	private Text _platformJumpText;

	private int _instructionIndex;
	private Animator _playerAnimator;
	private bool _showInstructionOverlay;



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
		// check has key
		_showInstructionOverlay = PlayerPrefs.GetInt ("showinstructionoverlay") == 1;
		//immediately revert the pref back to 0
		PlayerPrefs.SetInt ("showinstructionoverlay", 0);

		_rightSideBoundaryRenderer = rightSideBoundary.GetComponent<LineRenderer> ();
		_rightSideBoundaryRenderer.sortingOrder = 2;
		_rightSideBoundaryRenderer.enabled = false;

		_leftSideBoundaryRenderer = leftSideBoundary.GetComponent<LineRenderer> ();
		_leftSideBoundaryRenderer.sortingOrder = 2;
		_leftSideBoundaryRenderer.enabled = false;

		_bribeCopText = bribeCopInstructions.GetComponent<Text> ();
		_bribeCopText.enabled = false;
		_platformJumpText = platformJumpInstructions.GetComponent<Text> ();
		_platformJumpText.enabled = false;
		_bribeCopRenderer = bribeCop.GetComponent<SpriteRenderer> ();
		_bribeCopRenderer.enabled = false;
		_platformJumpRenderer = platformJump.GetComponent<SpriteRenderer> ();
		_platformJumpRenderer.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {

		if (PlayerPrefs.HasKey ("leaderboards") && PlayerPrefs.HasKey ("showinstructionoverlay")) {
			List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));

			if (leaderboards.Count < 100 && _showInstructionOverlay && 
				(_instructionIndex == 0 || (playerController.level2Start && _instructionIndex == 5))) {
				playerController.moveSpeed = 1f;
				_playerAnimator.speed = 1f / 3f;
				_instructionIndex++;

			}

		} else {

			playerController.moveSpeed = 1f;
			_playerAnimator.speed = 1f / 3f;
			_instructionIndex = 1;

		}

		float showDuration = 3f;
		float fadeDuration = 0.5f;

		// only go to the next instruction once all texts are disabled
		if (_rightSide.enabled == false && _leftSide.enabled == false &&
		    _avoidText.enabled == false && _instructionIndex < 5 && _instructionIndex > 0) {

			switch (_instructionIndex) {
			case 1:
				// tap the right side to jump
				_rightSide.enabled = true;
				_rightSideBoundaryRenderer.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_rightSide, showDuration, fadeDuration));
				StartCoroutine (Utility.FadeLineOut (_rightSideBoundaryRenderer, showDuration, fadeDuration));
				break;
			case 2:
				// hold the left side to slide
				_leftSide.enabled = true;
				_leftSideBoundaryRenderer.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_leftSide, showDuration, fadeDuration));
				StartCoroutine (Utility.FadeLineOut (_leftSideBoundaryRenderer, showDuration, fadeDuration));
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

		} else if (playerController.levelUpText.enabled == false && playerController.levelUpSubtitle.enabled == false && 
			_bribeCopText.enabled == false && _bribeCopRenderer.enabled == false && 
			_platformJumpText.enabled == false && _platformJumpRenderer.enabled == false && 
			playerController.level2Start && _instructionIndex < 10) {
			
			switch (_instructionIndex) {
			case 6:
				// show the level 2 intro text
				playerController.levelUpText.enabled = true;
				playerController.levelUpSubtitle.enabled = true;
				StartCoroutine (Utility.FadeTextOut (playerController.levelUpText, 6f, 0.5f));
				StartCoroutine (Utility.FadeTextOut (playerController.levelUpSubtitle, 6f, 0.5f));
				break;
			case 7:
				_bribeCopRenderer.enabled = true;
				_bribeCopText.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_bribeCopText, showDuration, fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_bribeCopRenderer, showDuration, fadeDuration));
				break;
			case 8:
				_platformJumpRenderer.enabled = true;
				_platformJumpText.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_platformJumpText, showDuration, fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_platformJumpRenderer, showDuration, fadeDuration));
				break;
			case 9:
				playerController.moveSpeed = 3f;
				_playerAnimator.speed = 1f;
				break;
			}

			_instructionIndex++;

		}
	}

}
