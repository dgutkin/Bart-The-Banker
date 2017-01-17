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
	public Transform groundCheck;
	public GameObject playerRespawn;
	public Text scoreText;
	public GameObject[] redHearts;
	public GameObject[] greyHearts;

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

		UpdateScore (0);
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
				ChangeCollider (true, true);
				_playerRigidbody.velocity = new Vector2 (moveSpeed, _playerRigidbody.velocity.y);
			}
		} else if (_unslide) {
			
			_unslide = false;
			_playerAnimator.SetBool ("Slide", false);
			_playerAnimator.SetBool ("UnSlide", true);
			ChangeCollider (false, true);

	    } else if (_grounded) {

			_playerRigidbody.velocity = new Vector2 (moveSpeed, _playerRigidbody.velocity.y);

		}
			
	}

	void OnCollisionEnter2D(Collision2D other) {
		
		if (other.gameObject.CompareTag("Ground")) {
			_grounded = true;
		}

	}

	void ChangeCollider(bool slide, bool grounded) {
		
		float x = 0f;
		float y = 0f;
			
		if (slide) {
			x = 2.1f;
			y = 1.2f;
		} else {
			x = 0.65f;
			y = 2.2f;
		}

		_playerCollider.size = new Vector2 (x, y);
				
		if (slide || !grounded) {
			_playerGroundCollider.radius = 0.07f; // need a collider on slide to avoid corner collision
			_playerGroundCollider.offset = new Vector2 (0f, -0.6f);
		} else {
			_playerGroundCollider.radius = 0.07f;
			_playerGroundCollider.offset = new Vector2 (0f, -1.1f);
			// issue with circle collider where collision on jump with ground cancels force
		}

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

	private void UpdateScore(int newScore) {
		_score = newScore;
		scoreText.text = "$" + _score.ToString();
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
		UpdateScore (_score + 10);
		_cash += 10;
	}

	public void HitDoubleBill() {
		UpdateScore (_score + 20);
		_cash += 20;
	}

	public void HitSingleStack() {
		UpdateScore (_score + 50);
		_cash += 50;
	}

	public void HitDoubleStack() {
		UpdateScore (_score + 100);
		_cash += 100;
	}

	public void HitCashBriefcase() {
		UpdateScore (_score + 500);
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
		UpdateScore (Mathf.RoundToInt(_score * 0.8f));
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
