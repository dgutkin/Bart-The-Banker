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
	private Material _mat;
	private bool _grounded = true;
	private int _score;
	private bool _jump;
	private int _lives;
	private Vector2[] _heartPositions;

	// Use this for initialization
	void Start () {
		moveSpeed = 3f;
		jumpYForce = 650f;
		jumpXForce = 0f;

		_playerRigidbody = GetComponent<Rigidbody2D> ();
		_playerAnimator = GetComponent<Animator> ();
		_playerRigidbody.freezeRotation = true;
		_playerRenderer = GetComponent<Renderer> ();
		_mat = _playerRenderer.material;
		_jump = false;

		UpdateScore (0);
		UpdateLives (3);
	}
		
	// Update is called once per frame
	void Update () 	{
	
		//_grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		//_grounded = Physics2D.Raycast (transform.position, -Vector2.up, distToGround);
		if (Input.GetMouseButtonDown (0) && _grounded) {
			_jump = true;
		}

	}

	// FixedUpdate is called every time the physics changes
	void FixedUpdate() {
		
		if (_jump) {
			_playerAnimator.SetTrigger ("Jump");
			_playerRigidbody.AddForce (new Vector2 (jumpXForce, jumpYForce));
			_jump = false;
			_grounded = false;
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
			break;
		}
	}

	public void hitBill() {
		UpdateScore (_score + 10);
	}

	public void hitDeathBlock() {
		//transform.position = playerRespawn.transform.position;
		//transform.rotation = Quaternion.identity;
		//_playerRigidbody.velocity = Vector2.zero;

		//updateScore (0);
		StartCoroutine("collideFlash");

		//Lose a life
		UpdateLives (_lives - 1);
		if (_lives == 0) {
			//end game
			SceneManager.LoadScene(1);
		}
	}

	IEnumerator collideFlash() {

		for (int i = 0; i < 5; i++) {
			_playerRenderer.material = null;
			yield return new  WaitForSeconds (0.1f);
			_playerRenderer.material = _mat;
			yield return new WaitForSeconds (0.1f);
		}
	}

}
