using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

	public float moveSpeed;
	public float jumpYForce;
	public float jumpXForce;
	private bool jump;

	public Transform groundCheck;

	private Rigidbody2D playerRigidbody;
	private Animator playerAnimator;
	private Renderer playerRenderer;
	private Material mat;
	private bool grounded = true;

	public GameObject playerRespawn;

	public Text scoreText;
	private int score;

	// Use this for initialization
	void Start () {
		playerRigidbody = GetComponent<Rigidbody2D> ();
		playerAnimator = GetComponent<Animator> ();
		playerRigidbody.freezeRotation = true;
		playerRenderer = GetComponent<Renderer> ();
		mat = playerRenderer.material;

		moveSpeed = 3f;
		jumpYForce = 650f;
		jumpXForce = 0f;
		jump = false;

		updateScore (0);
	}
		
	// Update is called once per frame
	void Update () 	{
	
		//grounded = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		//grounded = Physics2D.Raycast (transform.position, -Vector2.up, distToGround);
		if (Input.GetMouseButtonDown (0) && grounded) {
			jump = true;
		}

	}

	// FixedUpdate is called every time the physics changes
	void FixedUpdate() {
		
		if (jump) {
			playerAnimator.SetTrigger ("Jump");
			playerRigidbody.AddForce (new Vector2 (jumpXForce, jumpYForce));
			jump = false;
			grounded = false;
		} else if (grounded) {
			//playerRigidbody.velocity = new Vector2 (moveSpeed, playerRigidbody.velocity.y);
		}

	}

	void OnCollisionEnter2D(Collision2D other) {
		
		if (other.gameObject.CompareTag("Ground")) {
			grounded = true;
		}

	}

	//void OnCollision2DExit(Collision2D other) {
	//	if (other.gameObject.tag == "Ground") {
	//		grounded = false;
	//	}
	//}

//	public void hitGavel() {
//		
//		transform.position = playerRespawn.transform.position;
//		transform.rotation = Quaternion.identity;
//		playerRigidbody.velocity = Vector2.zero;
//
//	}

	private void updateScore(int newScore) {
		score = newScore;
		scoreText.text = "Score: " + score.ToString();
	}

	public void hitBill() {
		updateScore (score + 10);
	}

	public void hitDeathBlock() {
		//transform.position = playerRespawn.transform.position;
		//transform.rotation = Quaternion.identity;
		//playerRigidbody.velocity = Vector2.zero;

		//updateScore (0);
		StartCoroutine("collideFlash");
		//SceneManager.LoadScene(1);
	}

	IEnumerator collideFlash() {

		for (int i = 0; i < 5; i++) {
			playerRenderer.material = null;
			yield return new  WaitForSeconds (0.1f);
			playerRenderer.material = mat;
			yield return new WaitForSeconds (0.1f);
		}
	}

}
