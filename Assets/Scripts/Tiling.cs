using UnityEngine;
using System.Collections;

 [RequireComponent (typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour { // attach as component to foreground/background object
	
	public float offsetX = 0.2f;

	public bool hasRight = false;
	public bool hasLeft = false;

	public bool reverseScale = false; // if object not tilable

	private float _spriteWidth = 0f;
	private Camera _cam;
	private Transform _myTransform;
	private float secondsUntilDestroy = 30f;

	void Awake() {
		
		_cam = Camera.main;
		_myTransform = transform;

	}

	// Use this for initialization
	void Start () {
		
		SpriteRenderer sRenderer = GetComponent<SpriteRenderer> ();
		_spriteWidth = sRenderer.sprite.bounds.size.x;

	}
	
	// Update is called once per frame
	void Update () {
		
		if (hasLeft == false || hasRight == false) {
			
			float camHorizontalExtend = _cam.orthographicSize * Screen.width / Screen.height;  // half of camera width
			float edgeVisiblePositionRight = (_myTransform.position.x + _spriteWidth / 2) - camHorizontalExtend;
			float edgeVisiblePositionLeft = (_myTransform.position.x - _spriteWidth / 2) + camHorizontalExtend;

			if (_cam.transform.position.x >= edgeVisiblePositionRight - offsetX && hasRight == false) {
				
				makeNewGround (1);
				hasRight = true;

			} else if (_cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && hasLeft == false) {
				
				makeNewGround (-1);
				hasLeft = true;

			}

		}

	}

	void makeNewGround (int direction) {

		// calculate position and make new tile
		Vector3 newPosition = new Vector3 (_myTransform.position.x + _spriteWidth * direction, _myTransform.position.y);
		Transform newGround = Instantiate (_myTransform, newPosition, _myTransform.rotation) as Transform;
		// destroy the new ground after 30 seconds to save space
		Destroy (newGround.gameObject, secondsUntilDestroy);

		if (reverseScale == true) {  // if not tileable reverse x scale
			
			newGround.localScale = new Vector3 (newGround.localScale.x * -1, newGround.localScale.y, newGround.localScale.z);

		}

		newGround.parent = _myTransform.parent;

		if (direction > 0) {
			
			newGround.GetComponent<Tiling> ().hasLeft = true;
			hasRight = true;

		} else {
			
			newGround.GetComponent<Tiling> ().hasRight = true;
			hasLeft = true;

		}

	}

}