using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	public float jumpYForce;
	public float jumpXForce;
	public float standColliderWidth;
	public float standColliderHeight;
	public float slideColliderWidth;
	public float slideColliderHeight;
	public float standColliderOffsetX;
	public float standColliderOffsetY;
	public float standGroundColliderOffsetY;
	public float slideColliderOffsetX;
	public float slideColliderOffsetY;
	public float slideGroundColliderOffsetY;
	public float jumpColliderWidth;
	public float jumpColliderHeight;
	public float jumpColliderOffsetX;
	public float jumpColliderOffsetY;
	public float jumpGroundColliderOffsetY;
	public GameObject playerRespawn;
	public Text scoreText;
	public Text popUpText;
	public Text levelUpText;
	public Text levelUpSubtitle;
	public GameObject[] redHearts;
	public GameObject[] greyHearts;
	public Canvas canvas;
	public float copWalkingSpeed;

	private Rigidbody2D _playerRigidbody;
	private Animator _playerAnimator;
	private Renderer _playerRenderer;
	private Transform _playerTransform;
	private BoxCollider2D _playerCollider;
	private CircleCollider2D _playerGroundCollider;
	private Material _mat;
	private bool _grounded = true;
	private int _score;
	private int _cash;
	private int _tax;
	private bool _jump;
	private int _lives;
	private Vector2[] _heartPositions;
	private bool _immortality;
	private bool _slide;
	private bool _unslide;
	private int _maxLives = 3;
	private bool _isPaused;
	private Vector3 _positionOffsetForRaycast;
	private float _touchExtendFactor;

	// Use this for initialization
	void Start () {
		
		moveSpeed = 3f;
		jumpYForce = 650f;
		jumpXForce = 0f;
		standColliderWidth = 0.25f; //0.25
		standColliderHeight = 2.2f; //2.2
		standColliderOffsetX = -0.3f; //-0.3
		standColliderOffsetY = 1.1f; //1.1
		standGroundColliderOffsetY = 0f; //0
		slideColliderWidth = 1.5f;
		slideColliderHeight = 1f;
		slideColliderOffsetX = -0.75f;
		slideColliderOffsetY = 0.5f;
		slideGroundColliderOffsetY = 0f;
		jumpColliderWidth = 0.5f; //0.8
		jumpColliderHeight = 1.4f;
		jumpColliderOffsetX = -0.5f; //-0.75
		jumpColliderOffsetY = 1f;
		jumpGroundColliderOffsetY = 0.3f;
		levelUpText.enabled = false;
		levelUpSubtitle.enabled = false;

		_playerRigidbody = GetComponent<Rigidbody2D> ();
		_playerAnimator = GetComponent<Animator> ();
		_playerRigidbody.freezeRotation = true;
		_playerRenderer = GetComponent<Renderer> ();
		_playerTransform = GetComponent<Transform> ();
		_playerCollider = GetComponent<BoxCollider2D> ();
		_playerGroundCollider = GetComponent<CircleCollider2D> ();
		_mat = _playerRenderer.material;
		_jump = false;
		_immortality = false;
		_slide = false;

		_cash = 0;
		_tax = 0;

		_score = 0;
		UpdateScore (0, false);
		UpdateLives (3, false);

		_isPaused = false;

		_positionOffsetForRaycast = new Vector3 (-1f, 0.5f, 0f);
		_touchExtendFactor = 1.5f;

	}

	void OnEnable() {
		PauseBehaviour.OnPauseChange += PauseChange;
		GameItemGenerator.OnGameSpeedChange += GameSpeedChange;
		CopBehaviour.OnBribe += Bribe;
	}
	void OnDisable() {
		PauseBehaviour.OnPauseChange -= PauseChange;
		GameItemGenerator.OnGameSpeedChange -= GameSpeedChange;
		CopBehaviour.OnBribe -= Bribe;
	}
	void PauseChange() {
		_isPaused = !_isPaused;
	}
	void GameSpeedChange(int level) {

		string subtitleMsg = "";

		// Begin speeding up the game at Level 3
		moveSpeed  = 3f + 0.5f * Math.Max(level - 2,0);
		jumpYForce = 650;
		// Slow down the cops
		// Half the speed every level
		copWalkingSpeed = 2.5f * Mathf.Pow(0.6f, Mathf.Max(level - 2, 0));

		// adjust the xForce and gravity to keep the same jump arc
		switch (level) {
		case 2:
			subtitleMsg = "WATCH OUT FOR THE COPS! TAP TO BRIBE AND STOP ONE!";
			break;
		case 3:
			jumpXForce = -10;
			_playerRigidbody.gravityScale = 2.5f;
			subtitleMsg = "MORE COPS, START RUNNING FASTER!";
			break;
		case 4:
			jumpXForce = -20;
			_playerRigidbody.gravityScale = 2.7f;
			subtitleMsg = "THE FASTER YOU RUN, THE FASTER YOU EARN.";
			break;
		case 5:
			jumpXForce = -30;
			_playerRigidbody.gravityScale = 2.75f;
			subtitleMsg = "ARE YOU A RUNNER OR A BANKER?";
			break;
		case 6:
			jumpXForce = -50;
			_playerRigidbody.gravityScale = 2.765f;
			subtitleMsg = "THEY SHOULD CALL YOU BART THE RUNNER, HOW LONG CAN YOU LAST?";
			break;
		}

		levelUpText.text = "LEVEL " + level;
		levelUpSubtitle.text = subtitleMsg;
		levelUpText.enabled = true;
		levelUpSubtitle.enabled = true;

		StartCoroutine (Utility.FadeTextOut (levelUpText, 6f, 0.5f));
		StartCoroutine (Utility.FadeTextOut (levelUpSubtitle, 6f, 0.5f));
	}
		
	// Update is called once per frame
	void Update () 	{

		if (_isPaused) {
			// when paused disable all controls
		} else {
			
			#if UNITY_EDITOR
				if (Input.GetKeyDown (KeyCode.UpArrow) && _grounded && 
					Physics2D.Raycast(_playerTransform.position + _positionOffsetForRaycast, Vector3.down, 1f,
					(1 << LayerMask.NameToLayer("Platform") | 1 << LayerMask.NameToLayer("Ground")))) {				
					// use a raycast to check if the player is sufficiently grounded to jump
						_jump = true;
						_grounded = false;

				} else if (Input.GetKey (KeyCode.DownArrow)) { // while user holds down the key
					_slide = true;
				} else if (Input.GetKeyUp (KeyCode.DownArrow)) {
					_unslide = true;
				}
			#endif

			//for touch input
			#if UNITY_ANDROID || UNITY_IOS
				if (Input.touchCount > 0) {
					Touch touch = Input.GetTouch (0);
					Vector3 touchPosition = Camera.main.ScreenToWorldPoint (touch.position);
					Vector3 cameraPosition = Camera.main.gameObject.transform.position;
					Vector2 touchPosition2D = new Vector2(touchPosition.x, touchPosition.y);

				Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition2D);
					
				if (hitCollider != null && hitCollider.CompareTag("Cop")) {

						// disble touch if cop is tapped
						} else if (touch.phase == TouchPhase.Began && touchPosition.x > cameraPosition.x && 
							touchPosition.y < cameraPosition.y * _touchExtendFactor && _grounded) { // one tap on the right half of screen
							_jump = true;
							_grounded = false;
						} else if ((touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
							&& touchPosition.x < cameraPosition.x && touchPosition.y < cameraPosition.y) {
							_slide = true;
						} else if (touch.phase == TouchPhase.Ended && touchPosition.x < cameraPosition.x) {
							_unslide = true;
						}
				
					}
					
			#endif
		}

	}

	// FixedUpdate is called every time the physics change
	void FixedUpdate() {

		if (_jump) {
			
			_jump = false;
			_playerAnimator.SetBool ("Run", false);
			_playerAnimator.SetBool ("UnSlide", false);
			_playerAnimator.SetTrigger ("Jump");
			_playerRigidbody.AddForce (new Vector2 (jumpXForce, jumpYForce));
			ChangeCollider (false, false);

		} else if (_slide) {

			_slide = false;

			if (_grounded) {

				_playerAnimator.SetBool("UnSlide", false);
				_playerAnimator.SetBool ("Run", false);
				_playerAnimator.SetBool ("Slide", true);
				
				_playerRigidbody.velocity = new Vector2 (moveSpeed, _playerRigidbody.velocity.y);

				ChangeCollider (true, true);

			}

		} else if (_unslide) {
			
			_unslide = false;
			_playerAnimator.SetBool ("Slide", false);
			_playerAnimator.SetBool ("UnSlide", true);

			ChangeCollider (false, true);

	    } else if (_grounded) {

			_playerAnimator.SetBool ("Run", true);
			_playerRigidbody.velocity = new Vector2 (moveSpeed, _playerRigidbody.velocity.y);

			if (!_playerAnimator.GetBool("Slide") && _playerAnimator.IsInTransition(0)) {

				ChangeCollider (false, true);

			}
		}

	}

	void OnCollisionEnter2D(Collision2D other) {

		if (other.gameObject.CompareTag ("Ground") && other.contacts.Length > 0) {

			// Disallow jumping when colliding with ceilings and corners
			// Normal vector is negative when colliding with platform from below
			ContactPoint2D contactPoint = other.contacts [0];
			if (Vector2.Dot (contactPoint.normal, Vector2.up) > 0.5) {
				_grounded = true;
			}

		}

	}

	void ChangeCollider(bool slide, bool grounded) {
		
		float sizeX;
		float sizeY;
		float offsetX;
		float offsetY;
		float groundOffsetY;
			
		if (slide) {
			
			sizeX = slideColliderWidth;
			sizeY = slideColliderHeight;
			offsetX = slideColliderOffsetX;
			offsetY = slideColliderOffsetY;
			groundOffsetY = slideGroundColliderOffsetY;

		} else if (grounded) {
			
			sizeX = standColliderWidth;
			sizeY = standColliderHeight;
			offsetX = standColliderOffsetX;
			offsetY = standColliderOffsetY;
			groundOffsetY = standGroundColliderOffsetY;

		} else {

			sizeX = jumpColliderWidth;
			sizeY = jumpColliderHeight;
			offsetX = jumpColliderOffsetX;
			offsetY = jumpColliderOffsetY;
			groundOffsetY = jumpGroundColliderOffsetY;

		}

		_playerCollider.size = new Vector2 (sizeX, sizeY);
		_playerCollider.offset = new Vector2 (offsetX, offsetY);
			
		_playerGroundCollider.offset = new Vector2 (offsetX, groundOffsetY);
		
	}

	public IEnumerator ScoreChange(int scoreChange) {
		// Create a pop up of the score change
		Text popUp = Instantiate (popUpText, _playerTransform.position, Quaternion.identity) as Text;
		popUp.transform.SetParent (canvas.transform, false);
		popUp.transform.position = new Vector3(_playerTransform.position.x - 3.5f, _playerTransform.position.y + 1, 0);

		if (scoreChange > 0) {
			popUp.text = "+$" + scoreChange.ToString ();
			popUp.color = Color.green;
		} else {
			popUp.text = "-$" + Math.Abs(scoreChange).ToString ();
			popUp.color = Color.red;
		}

		// Flash score in top right
		Color flash = scoreChange > 0 ? Color.green : Color.red;
		for(int i = 0; i < 6; ++i) {
			scoreText.color = i % 2 == 0 ? flash : Color.white;
			yield return new WaitForSeconds(0.1f);
		}

		// This can be object pooled
		Destroy (popUp.gameObject);
	}

	private void UpdateScore(int scoreChange, bool animate = true) {
		_score += scoreChange;
		scoreText.text = "$" + _score.ToString();

		if (animate) {
			// Flash score in top right and make popup
			StartCoroutine (ScoreChange (scoreChange));
		}
	}


	public void LifeChange(int lifeChange) {
		
		Text popUp = Instantiate (popUpText, _playerTransform.position, Quaternion.identity) as Text;
		popUp.transform.SetParent (canvas.transform, false);
		popUp.transform.position = new Vector3(_playerTransform.position.x - 3.5f, _playerTransform.position.y + 1, 0);

		if (lifeChange > 0) {
			popUp.text = "+Life";
			popUp.color = Color.green;
		} else {
			popUp.text = "-Life";
			popUp.color = Color.red;
		}

		Destroy (popUp.gameObject, 3f);

	}

	private void UpdateLives(int lifeChange, bool animate = true) {
		
		if (animate) {
			// Make popup
			LifeChange (lifeChange);
		}

		_lives = Math.Min(_lives + lifeChange, _maxLives);
		switch (_lives) {
		case 3:
			for (int i = 0; i < greyHearts.Length; ++i) {
				redHearts [i].SetActive (true);
				greyHearts [i].SetActive (false);
			}
			break;
		case 2:
			redHearts [0].SetActive (true);
			redHearts [1].SetActive (true);
			redHearts [2].SetActive (false);
			greyHearts [2].SetActive (true);
			break;
		case 1:
			redHearts [0].SetActive (true);
			redHearts [1].SetActive (false);
			redHearts [2].SetActive (false);
			greyHearts [1].SetActive (true);
			greyHearts [2].SetActive (true);
			break;
		case 0:
			for (int i = 0; i < greyHearts.Length; ++i) {
				redHearts [i].SetActive (false);
				greyHearts [i].SetActive (true);
			}

			//Save highscore if top 20
			int highscore = 0;
			if (PlayerPrefs.HasKey ("leaderboards")) {
				List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));

				for (int i = 1; i < leaderboards.Count; i += 2) {
					if (_score >= Int32.Parse (leaderboards [i])) {
						i--;
						leaderboards.Insert (i, _score.ToString ());
						leaderboards.Insert (i, System.DateTime.Now.ToString ("MM/dd/yyyy"));
						break;
					}
				}

				int endRange = leaderboards.Count > 40 ? 40 : leaderboards.Count;
				PlayerPrefs.SetString ("leaderboards", String.Join (";", leaderboards.GetRange (0, endRange).ToArray ()));
				highscore = Int32.Parse (leaderboards [1]);
			} else {
				//Add to playerprefs
				PlayerPrefs.SetString ("leaderboards", System.DateTime.Now.ToString ("MM/dd/yyyy") + ";" + _score.ToString ());
				highscore = _score;
			}
			PlayerPrefs.SetInt ("score", _score);
			PlayerPrefs.SetInt ("cash", _cash);
			PlayerPrefs.SetInt ("tax", _tax);
			PlayerPrefs.Save ();

			//End game screen
			SceneManager.LoadScene("GameOver");
			break;
		}

	}

	public void HitHeart() {
		UpdateLives (1);
	}

	public void HitSingleBill() {
		UpdateScore (10);
		_cash += 10;
	}

	public void HitDoubleBill() {
		UpdateScore (20);
		_cash += 20;
	}

	public void HitSingleStack() {
		UpdateScore (50);
		_cash += 50;
	}

	public void HitDoubleStack() {
		UpdateScore (100);
		_cash += 100;
	}

	public void HitCashBriefcase() {
		UpdateScore (500);
		_cash += 500;
	}

	public void HitObstacle() {

		//Lose a life
		if (!_immortality) {
			AudioSource audio = GetComponent<AudioSource> ();
			audio.Play ();
			UpdateLives (-1);
		}

		StartCoroutine("CollideFlash");
	}

	public void HitTaxBlock() {
		
		_tax += Mathf.RoundToInt (_score * 0.2f);
		UpdateScore (Mathf.RoundToInt(-1 * _score * 0.2f));

	}

	IEnumerator CollideFlash() {
		
		_immortality = true;
		for (int i = 0; i < 5; i++) {
			_playerRenderer.material = null;
			yield return new WaitForSeconds (0.1f);
			_playerRenderer.material = _mat;
			yield return new WaitForSeconds (0.1f);
		}
		_immortality = false;

	}

	public void Bribe() {

		int charge = Mathf.Min (100, _score);
		_tax += charge;
		UpdateScore (-1 * charge);

	}

}
