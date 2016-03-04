﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float moveSpeed = 1f;
	public float jumpYForce = 600f;
	public float jumpXForce = 100f;
	public bool jump = false;

	public Transform groundCheck;

	private Rigidbody2D playerRigidbody;
	private bool grounded = true;

	public GameObject playerRespawn;

	// Use this for initialization
	void Start () {
		playerRigidbody = GetComponent<Rigidbody2D> ();
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
			playerRigidbody.AddForce (new Vector2 (jumpXForce, jumpYForce));
			jump = false;
			grounded = false;
		} else if (grounded) {
			playerRigidbody.velocity = new Vector2 (moveSpeed, playerRigidbody.velocity.y);
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

	//  need camera tracker script ... safer than just making camera child of player
	//  actually having the camera as a child of the player is ok as long as the ground always persists (can't fall off)

	public void hitGavel() {
		
		transform.position = playerRespawn.transform.position;
		transform.rotation = Quaternion.identity;
		playerRigidbody.velocity = Vector2.zero;

	}

}
