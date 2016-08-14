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
	private Material _mat;
	private bool _grounded = true;
	private int _score;
	private bool _jump;
	private int _lives;
	private Vector2[] _heartPositions;
	private bool _immortality;
	private bool _slide;
	private bool _unslide;

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
		_mat = _playerRenderer.material;
		_jump = false;
		_immortality = false;
		_slide = false;

		UpdateScore (0);
		UpdateLives (3);
	}
		
	// Update is called once per frame
	void Update () 	{
	
		//_grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		//_grounded = Physics2D.Raycast (transform.position, -Vector2.up, distToGround);
		if (Input.GetKeyDown (KeyCode.UpArrow) && _grounded) {
			_jump = true;
		} else if (Input.GetKeyDown (KeyCode.DownArrow) && _grounded) {
			_slide = true;
		} else if (Input.GetKeyUp (KeyCode.DownArrow) && _grounded) {
			_unslide = true;
		}

	}

	// FixedUpdate is called every time the physics changes
	void FixedUpdate() {
		
		if (_jump) {
			_playerAnimator.SetTrigger ("Jump");
			_playerRigidbody.AddForce (new Vector2 (jumpXForce, jumpYForce));
			_jump = false;
			_grounded = false;
		} else if (_slide) {
			//StartCoroutine (Slide ());
			_playerCollider.size = new Vector2 (0.3f, 0.15f);
			_playerTransform.Translate (0f, -0.5f, 0f);
			_playerAnimator.SetTrigger ("Slide");
			_slide = false;
		} else if (_unslide) {
			_playerCollider.size = new Vector2(0.15f, 0.3f);
			_playerTransform.Translate (0f, 0.5f, 0f);
			_playerAnimator.SetTrigger ("UnSlide");
			_unslide = false;
	    } else if (_grounded) {
			_playerRigidbody.velocity = new Vector2 (moveSpeed, _playerRigidbody.velocity.y);
		}

	}

	void OnCollisionEnter2D(Collision2D other) {
		
		if (other.gameObject.CompareTag("Ground")) {
			_grounded = true;
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

			//Save highscore
			if (PlayerPrefs.HasKey ("leaderboards")) {
				List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split(';'));

				for (int i = 1; i < leaderboards.Count; i += 2) {
					if (_score >= Int32.Parse (leaderboards [i])) {
						i--;
						leaderboards.Insert (i, System.DateTime.Now.ToString ("MM/dd/yyyy") + ";" + _score.ToString ());
						break;
					}
				}

				PlayerPrefs.SetString ("leaderboards", String.Join (";", leaderboards.ToArray ()));
			} else {
				//Add to playerprefs
				PlayerPrefs.SetString ("leaderboards", System.DateTime.Now.ToString ("MM/dd/yyyy") + ";" + _score.ToString ());
			}
			PlayerPrefs.Save ();

			//End game
			SceneManager.LoadScene(0);
			break;
		}
	}

	public void HitBill() {
		UpdateScore (_score + 10);
	}

	public void HitObstacle() {
		//transform.position = playerRespawn.transform.position;
		//transform.rotation = Quaternion.identity;
		//_playerRigidbody.velocity = Vector2.zero;

		//Lose a life
		if (!_immortality) {
			UpdateLives (_lives - 1);
		}

		StartCoroutine("CollideFlash");
	}

	public void HitTaxBlock() {
		UpdateScore (Mathf.RoundToInt(_score * 0.8f));
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
