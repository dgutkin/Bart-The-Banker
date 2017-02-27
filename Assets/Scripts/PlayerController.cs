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
	public float slideColliderOffsetX;
	public float slideColliderOffsetY;
	public float jumpColliderWidth;
	public float jumpColliderHeight;
	public float jumpColliderOffsetX;
	public float jumpColliderOffsetY;
	public Transform groundCheck;
	public GameObject playerRespawn;
	public Text scoreText;
	public Text popUpText;
	public GameObject[] redHearts;
	public GameObject[] greyHearts;
	public Canvas canvas;

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

	public GameObject background;
	public AudioClip painClip;

	// Use this for initialization
	void Start () {
		
		moveSpeed = 3f;
		jumpYForce = 650f;
		jumpXForce = 0f;
		standColliderWidth = 0.25f;
		standColliderHeight = 2.2f;
		slideColliderWidth = 2f;
		slideColliderHeight = 1f;
		standColliderOffsetX = -0.3f;
		standColliderOffsetY = 1.1f;
		slideColliderOffsetX = -1f;
		slideColliderOffsetY = 0.5f;
		jumpColliderWidth = 0.8f;
		jumpColliderHeight = 1.75f;
		jumpColliderOffsetX = -0.75f;
		jumpColliderOffsetY = 0.87f;

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
		UpdateLives (3);

	}
		
	// Update is called once per frame
	void Update () 	{
		
		//_grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		//_grounded = Physics2D.Raycast (transform.position, -Vector2.up, distToGround);
		#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.UpArrow) && _grounded) {
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
			if (touch.phase == TouchPhase.Began && touchPosition.x > cameraPosition.x) { // one tap on the right half of screen
				_jump = true;
				_grounded = false;
			} else if ((touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
			           && touchPosition.x < cameraPosition.x) {
				_slide = true;
			} else if (touch.phase == TouchPhase.Ended && touchPosition.x < cameraPosition.x) {
				_unslide = true;
			}
		}
		#endif

	}

	// FixedUpdate is called every time the physics changes
	void FixedUpdate() {

		if (_jump) {
			
			_jump = false;
			_playerAnimator.SetTrigger ("Jump");
			_playerRigidbody.AddForce (new Vector2 (jumpXForce, jumpYForce));
			ChangeCollider (false, false);

		} else if (_slide) {

			_slide = false;
			_playerAnimator.SetBool("UnSlide", false);
			_playerAnimator.SetBool ("Slide", true);

			if (_grounded) {
				
				_playerRigidbody.velocity = new Vector2 (moveSpeed, _playerRigidbody.velocity.y);

				if (_playerAnimator.IsInTransition (0) &&
				    _playerAnimator.GetNextAnimatorStateInfo (0).shortNameHash == Animator.StringToHash("Slide")) {

					ChangeCollider (true, true);

				}
			}

		} else if (_unslide) {
			
			_unslide = false;
			_playerAnimator.SetBool ("Slide", false);
			_playerAnimator.SetBool ("UnSlide", true);

			if (_playerAnimator.IsInTransition(0) && 
				_playerAnimator.GetNextAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Run")) {

				ChangeCollider (false, true);

			}

	    } else if (_grounded) {

			_playerRigidbody.velocity = new Vector2 (moveSpeed, _playerRigidbody.velocity.y);

			if (!_playerAnimator.GetBool("Slide") && _playerAnimator.IsInTransition(0)) {

				ChangeCollider (false, true);

			}
		}
			
	}

	void OnCollisionEnter2D(Collision2D other) {
		
		if (other.gameObject.CompareTag("Ground")) {
			_grounded = true;
		}

	}

	void ChangeCollider(bool slide, bool grounded) {
		
		float sizeX;
		float sizeY;
		float offsetX;
		float offsetY;
			
		if (slide) {
			
			sizeX = slideColliderWidth;
			sizeY = slideColliderHeight;
			offsetX = slideColliderOffsetX;
			offsetY = slideColliderOffsetY;

		} else if (grounded) {
			
			sizeX = standColliderWidth;
			sizeY = standColliderHeight;
			offsetX = standColliderOffsetX;
			offsetY = standColliderOffsetY;

		} else {

			sizeX = jumpColliderWidth;
			sizeY = jumpColliderHeight;
			offsetX = jumpColliderOffsetX;
			offsetY = jumpColliderOffsetY;

		}

		_playerCollider.size = new Vector2 (sizeX, sizeY);
		_playerCollider.offset = new Vector2 (offsetX, offsetY);
			
		_playerGroundCollider.offset = new Vector2 (offsetX, 0f);
		
	}

	//void OnCollision2DExit(Collision2D other) {
	//	if (other.gameObject.tag == "Ground") {
	//		_grounded = false;
	//	}
	//}

//	public void hitGavel() {
//		
//		transform.position = playerRespawn.transform.position;
//		transform.rotation = Quaternion.identity;
//		_playerRigidbody.velocity = Vector2.zero;
//
//	}

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

		// To Do: This can be object pooled!!
		Destroy (popUp);
	}

	private void UpdateScore(int scoreChange, bool animate = true) {
		_score += scoreChange;
		scoreText.text = "$" + _score.ToString();

		if (animate) {
			// Flash score in top right and make popup
			StartCoroutine (ScoreChange (scoreChange));
		}
	}

	private void UpdateLives(int newLives) {
		_lives = newLives;
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
		//transform.position = playerRespawn.transform.position;
		//transform.rotation = Quaternion.identity;
		//_playerRigidbody.velocity = Vector2.zero;

		//Lose a life
		if (!_immortality) {
			AudioSource audio = GetComponent<AudioSource> ();
			audio.Play ();
			UpdateLives (_lives - 1);
		}

		StartCoroutine("CollideFlash");
	}

	public void HitTaxBlock() {
		UpdateScore (Mathf.RoundToInt(-1 * _score * 0.2f));
		_tax += Mathf.RoundToInt (_score * 0.2f);
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

//	IEnumerator Slide() {
//
//		_playerCollider.size = new Vector2 (0.3f, 0.15f);
//		_playerTransform.Translate (0f, -0.5f, 0f);
//		_playerAnimator.SetTrigger ("Slide");
//		yield return new WaitForSeconds (1.0f);  //need animation time of slide
//		_playerCollider.size = new Vector2(0.15f, 0.3f);
//		_playerTransform.Translate (0f, +0.5f, 0f);
//
//	}

}
