using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CopBehaviour : MonoBehaviour {

	public float walkingSpeed = 2.5f;
	public float leftTurnAroundDelay = 0f;
	public GameObject bribeLabelText;
	public Text insufficientFundsText;

	public delegate void BribeAction();
	public static event BribeAction OnBribe;

	private float _leftTurnAroundTime;
	private bool _walkingLeft;
	private float _distanceWalked;
	private float _leftBound;
	private float _rightBound;
	private Animator _copAnimator;

	private PlayerController _playerController;
	private BoxCollider2D _copCollider;
	private Animation _copAnimation;

	private float _secondsUntilDestroy = 30f;
	private float _bribeLabelHeightOffset = 1.2f;
	private bool touchInitiatedOnCop;

	// Use this for initialization
	void Start () {
		
		_distanceWalked = 0f;
		_walkingLeft = true;
		_leftBound = transform.position.x - 0.8f;
		_rightBound = transform.position.x + 1.5f;
		_copAnimator = GetComponent<Animator> ();

		_copCollider = GetComponent<BoxCollider2D> ();
		_copAnimation = GetComponent<Animation> ();
		UpdateWalkOrientation ();

		// set the right walking speed according to the level
		_playerController = GameObject.FindWithTag ("Player").GetComponent<PlayerController> ();
		walkingSpeed = _playerController.copWalkingSpeed;

	}
	
	// Update is called once per frame
	void Update () {

		#if UNITY_ANDROID || UNITY_IOS

		if (Input.touchCount > 1 && walkingSpeed > 0) {

			Touch touch = Input.GetTouch(0);
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint (touch.position);
			Vector2 touchPosition2D = new Vector2(touchPosition.x, touchPosition.y);
			Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition2D);

			Touch secondTouch = Input.GetTouch(1);
			Vector3 secondTouchPosition = Camera.main.ScreenToWorldPoint(secondTouch.position);
			Vector2 secondTouchPosition2D = new Vector2(secondTouchPosition.x, secondTouchPosition.y);
			Collider2D secondHitCollider = Physics2D.OverlapPoint(secondTouchPosition2D);

			if ((hitCollider != null && _copCollider.OverlapPoint(touchPosition2D) && 
				touchPosition2D.y < Constants.BRIBE_BOUNDARY) ||
				(secondHitCollider != null && _copCollider.OverlapPoint(secondTouchPosition2D) &&
					secondTouchPosition2D.y < Constants.BRIBE_BOUNDARY)) {  
				BribeCop();
			}

		} else if (Input.touchCount > 0 && walkingSpeed > 0) {

			Touch touch = Input.GetTouch(0);
			Vector3 touchPosition = Camera.main.ScreenToWorldPoint (touch.position);
			Vector2 touchPosition2D = new Vector2(touchPosition.x, touchPosition.y);
			Vector3 cameraPosition = Camera.main.gameObject.transform.position;
			Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition2D);

			if (hitCollider != null && _copCollider.OverlapPoint(touchPosition2D) &&
				touchPosition2D.y < Constants.BRIBE_BOUNDARY) {
				BribeCop();
			}

		}

		#endif

		if (walkingSpeed > 0) {
			if (Time.time > _leftTurnAroundTime) { // start walking right after delay
				
				// Just walk in one way, rotation dictates direction
				transform.Translate (Vector3.left * walkingSpeed * Time.deltaTime);

				if (_copAnimator.speed == 0) {
					_copAnimator.speed = 1; // resume animation
				}

			}
			// Swap directions once past bounds
			if (transform.position.x >= _rightBound || transform.position.x <= _leftBound) {
				
				// Snap position back to just behind before swapping directions
				float newX = transform.position.x >= _rightBound ? _rightBound - 0.001f : _leftBound + 0.001f;
				transform.position = new Vector3 (newX, transform.position.y, transform.position.z);
				SwitchDirections ();

			}
		}
	}

	public void SwitchDirections() {

		_walkingLeft = !_walkingLeft;
		UpdateWalkOrientation ();

	}

	void UpdateWalkOrientation() {
		
		if (_walkingLeft) {
			
			transform.localRotation = Quaternion.Euler (0, 180, 0);
			_leftTurnAroundTime = Time.time + leftTurnAroundDelay;
			_copAnimator.speed = 0; // stop the animation

		} else {
			
			transform.localRotation = Quaternion.Euler (0, 0, 0);

		}

	}

	void OnTriggerEnter2D(Collider2D otherObj) {
		
		if (otherObj.gameObject.CompareTag ("Player")) {
			
			otherObj.SendMessage ("HitObstacle", SendMessageOptions.DontRequireReceiver);

		}

	}

	void BribeCop() {

		if (_playerController._score > 100) {

			// freeze the cop once bribed
			walkingSpeed = 0;
			_copAnimator.speed = 0;

			GameObject bribeLabel = Instantiate (bribeLabelText, transform.position + new Vector3 (0, _bribeLabelHeightOffset, 0), Quaternion.identity);
			Destroy (bribeLabel, _secondsUntilDestroy);

			OnBribe ();

		} else {

			// insufficient funds to bribe
			Vector3 insufficientFundsMessagePosition = new Vector3 (
															Camera.main.gameObject.transform.position.x,
				                                           Camera.main.gameObject.transform.position.y, 0);
			Text insufficientFundsMessage = Instantiate (insufficientFundsText, 
				insufficientFundsMessagePosition, Quaternion.identity) as Text;
			insufficientFundsMessage.transform.SetParent (_playerController.canvas.transform, false);
			StartCoroutine(Utility.FadeTextOut(insufficientFundsMessage, 1f, 0.5f));

		}

	}

}