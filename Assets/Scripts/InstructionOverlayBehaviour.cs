using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

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

	public Text levelUpText;
	public Text levelUpSubtitle;

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
	private bool _level2Start;

	private float _playerMoveSpeed;
	private float _playerGravityScale;
	private float _playerJumpYForce;
	private float _playerJumpXForce;

	private float _showLevelDuration;
	private float _showInstructionDuration;
	private float _fadeDuration;

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

		_level2Start = false;

		levelUpText.enabled = false;
		levelUpSubtitle.enabled = false;

		_playerMoveSpeed = playerController.moveSpeed;
		_playerGravityScale = playerController.gravityScale;
		_playerJumpYForce = playerController.jumpYForce;
		_playerJumpXForce = playerController.jumpXForce;

		_showLevelDuration = 5f;
		_showInstructionDuration = 3f;
		_fadeDuration = 0.5f;

	}

	void OnEnable() {

		GameItemGenerator.OnLevelChange += LevelChange;

	}

	void OnDisable() {

		GameItemGenerator.OnLevelChange -= LevelChange;

	}
	
	// Update is called once per frame
	void Update () {

		if (PlayerPrefs.HasKey ("leaderboards") && PlayerPrefs.HasKey ("showinstructionoverlay")) {
			List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));

			if (leaderboards.Count < 2 && _showInstructionOverlay && 
				(_instructionIndex == 0 || (_level2Start && _instructionIndex == 5))) {
				playerController.moveSpeed = 1f;
				_playerAnimator.speed = 1f / 3f;
				_instructionIndex++;

			}

		} else {

			playerController.moveSpeed = 1f;
			_playerAnimator.speed = 1f / 3f;
			_instructionIndex = 1;

		}

		// only go to the next instruction once all texts are disabled
		if (_rightSide.enabled == false && _leftSide.enabled == false &&
		    _avoidText.enabled == false && _instructionIndex < 5 && _instructionIndex > 0) {

			switch (_instructionIndex) {
			case 1:
				// tap the right side to jump
				_rightSide.enabled = true;
				_rightSideBoundaryRenderer.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_rightSide, _showInstructionDuration, _fadeDuration));
				StartCoroutine (Utility.FadeLineOut (_rightSideBoundaryRenderer, _showInstructionDuration, _fadeDuration));
				break;
			case 2:
				// hold the left side to slide
				_leftSide.enabled = true;
				_leftSideBoundaryRenderer.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_leftSide, _showInstructionDuration, _fadeDuration));
				StartCoroutine (Utility.FadeLineOut (_leftSideBoundaryRenderer, _showInstructionDuration, _fadeDuration));
				break;
			case 3:
				// avoid the cages, taxes and cops
				_avoidText.enabled = true;
				_avoidCageRenderer.enabled = true;
				_avoidTaxRenderer.enabled = true;
				_avoidCopRenderer.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_avoidText, _showInstructionDuration, _fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_avoidCageRenderer, _showInstructionDuration, _fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_avoidTaxRenderer, _showInstructionDuration, _fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_avoidCopRenderer, _showInstructionDuration, _fadeDuration));
				break;
			case 4:
				playerController.moveSpeed = 3f;
				_playerAnimator.speed = 1f;
				break;
			}

			_instructionIndex++;
		} else if (levelUpText.enabled == false && levelUpSubtitle.enabled == false && 
			_bribeCopText.enabled == false && _bribeCopRenderer.enabled == false && 
			_platformJumpText.enabled == false && _platformJumpRenderer.enabled == false && 
			_level2Start && (_instructionIndex == 0 || _instructionIndex == 5)) {

			// show the level 2 intro text
			levelUpText.enabled = true;
			levelUpSubtitle.enabled = true;
			StartCoroutine (Utility.FadeTextOut (levelUpText, _showLevelDuration, _fadeDuration));
			StartCoroutine (Utility.FadeTextOut (levelUpSubtitle, _showLevelDuration, _fadeDuration));
			if (_instructionIndex == 5) {
				_instructionIndex++;
			} else if (_instructionIndex == 0) {
				_instructionIndex = 9;
			}

		} else if (levelUpText.enabled == false && levelUpSubtitle.enabled == false && 
			_bribeCopText.enabled == false && _bribeCopRenderer.enabled == false && 
			_platformJumpText.enabled == false && _platformJumpRenderer.enabled == false && 
			_level2Start && _instructionIndex < 9 && _instructionIndex > 5) {
			
			switch (_instructionIndex) {
			case 6:
				_bribeCopRenderer.enabled = true;
				_bribeCopText.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_bribeCopText, _showInstructionDuration, _fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_bribeCopRenderer, _showInstructionDuration, _fadeDuration));
				break;
			case 7:
				_platformJumpRenderer.enabled = true;
				_platformJumpText.enabled = true;
				StartCoroutine (Utility.FadeTextOut (_platformJumpText, _showInstructionDuration, _fadeDuration));
				StartCoroutine (Utility.FadeSpriteOut (_platformJumpRenderer, _showInstructionDuration, _fadeDuration));
				break;
			case 8:
				playerController.moveSpeed = 3f;
				_playerAnimator.speed = 1f;
				break;
			}

			_instructionIndex++;

		}
			
	}

	void LevelChange(int level) {

		string subtitleMsg = "";

		// Begin speeding up the game at Level 3
		_playerMoveSpeed  = 3f + 0.5f * Math.Max(level - 2,0);
		_playerJumpYForce = 650;

		// adjust the xForce and gravity to keep the same jump arc
		switch (level) {
		case 2:
			subtitleMsg = "WATCH OUT FOR THE COPS! TAP TO BRIBE!";
			_level2Start = true;
			break;
		case 3:
			_playerJumpXForce = -10;
			_playerGravityScale = 2.5f;
			subtitleMsg = "MORE COPS, START RUNNING FASTER!";
			break;
		case 4:
			_playerJumpXForce = -20;
			_playerGravityScale = 2.7f;
			subtitleMsg = "THE FASTER YOU RUN, THE FASTER YOU EARN!";
			break;
		case 5:
			_playerJumpXForce = -30;
			_playerGravityScale = 2.75f;
			subtitleMsg = "THEY SHOULD CALL YOU BART THE RUNNER?";
			break;
		case 6:
			_playerJumpXForce = -50;
			_playerGravityScale = 2.765f;
			subtitleMsg = "HOW LONG CAN YOU LAST?";
			break;
		}

		levelUpText.text = "LEVEL " + level;
		levelUpSubtitle.text = subtitleMsg;

		if (level > 2) {
			levelUpText.enabled = true;
			levelUpSubtitle.enabled = true;

			StartCoroutine (Utility.FadeTextOut (levelUpText, _showLevelDuration, _fadeDuration));
			StartCoroutine (Utility.FadeTextOut (levelUpSubtitle, _showLevelDuration, _fadeDuration));
		}

		// update variables in PlayerController
		playerController.moveSpeed = _playerMoveSpeed;
		playerController.gravityScale = _playerGravityScale;
		playerController.jumpYForce = _playerJumpYForce;
		playerController.jumpXForce = _playerJumpXForce;

	}

}
