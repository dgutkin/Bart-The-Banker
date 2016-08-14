using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
	private bool _inslide;

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
		_inslide = false;

		UpdateScore (0);
		UpdateLives (3);
	}
		
	// Update is called once per frame
	void Update () 	{
		
		//_grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		//_grounded = Physics2D.Raycast (transform.position, -Vector2.up, distToGround);
		if (Input.GetKeyDown (KeyCode.UpArrow) && _grounded) {
			if (_inslide) {
				_unslide = true;
				_inslide = false;
			}
			_jump = true;
			_grounded = false;
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) { // dont need _grounded so slide can be queued mid jump (wait)
			_slide = true;
			_inslide = true;
		} else if (Input.GetKeyUp (KeyCode.DownArrow)) {
			_unslide = true;
			_inslide = false;
		}

		if (_inslide && _grounded && (_playerCollider.size.x != 0.37f || _playerCollider.size.y != 0.15f)) {
			_playerCollider.size = new Vector2 (0.37f, 0.17f);
			// ensure collider is right size when sliding and grounded
		}

	}

	// FixedUpdate is called every time the physics changes
	void FixedUpdate() { // slide then jump then slide produces bug (try to use direct animation from jump to slide)
		
		if (_jump) {
			_playerAnimator.SetTrigger ("Jump");
			_playerRigidbody.AddForce (new Vector2 (jumpXForce, jumpYForce));
			_jump = false;
			//_grounded = false;
		} else if (_slide) {
			//StartCoroutine (Slide ());
			_playerAnimator.SetTrigger ("Slide");
			if (_grounded) {
				_playerCollider.size = new Vector2 (0.37f, 0.17f);
				_playerTransform.Translate (0f, -0.0f, 0f);
			}

			_slide = false;
		} else if (_unslide) {
			_playerAnimator.SetTrigger ("UnSlide");
			if (_grounded) {
				_playerCollider.size = new Vector2 (0.17f, 0.37f);
				_playerTransform.Translate (0f, 0.0f, 0f);
			}
			_unslide = false;
			Debug.Log ("UNSLIDE!");
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

			//end game
			SceneManager.LoadScene(1);
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
