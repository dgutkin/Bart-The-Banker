using UnityEngine;
using System.Collections;

 [RequireComponent (typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour { // attach as component to foreground/background object
	
	public float offsetX = 0.2f;

	public bool hasRight = false;
	public bool hasLeft = false;

	public bool reverseScale = false; // if object not tilable

	private float spriteWidth = 0f;
	private Camera cam;
	private Transform myTransform;

	void Awake() {
		cam = Camera.main;
		myTransform = transform;
	}

	// Use this for initialization
	void Start () {
		SpriteRenderer sRenderer = GetComponent<SpriteRenderer> ();
		spriteWidth = sRenderer.sprite.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
		if (hasLeft == false || hasRight == false) {
			float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;  // half of camera width
			float edgeVisiblePositionRight = (myTransform.position.x + spriteWidth / 2) - camHorizontalExtend;
			float edgeVisiblePositionLeft = (myTransform.position.x - spriteWidth / 2) + camHorizontalExtend;
			Debug.Log ("Cam" + camHorizontalExtend.ToString ());
			Debug.Log ("Left" + edgeVisiblePositionLeft.ToString());
			Debug.Log ("Right" + edgeVisiblePositionRight.ToString());
			if (cam.transform.position.x >= edgeVisiblePositionRight - offsetX && hasRight == false) {
				makeNewGround (1);
				hasRight = true;
			} else if (cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && hasLeft == false) {
				makeNewGround (0);
				hasLeft = true;
			}
		} // only need to make new ground in one direction at a time

	}

	void makeNewGround (int direction) {
		Vector3 newPosition = new Vector3 (myTransform.position.x + spriteWidth * direction, myTransform.position.y);
		Transform newGround = Instantiate (myTransform, newPosition, myTransform.rotation) as Transform;
		Destroy (newGround.gameObject, 30.0f); // destroy the new ground after 30 seconds to save space
		// when player loses all lives respawn and restart background tiling

		if (reverseScale == true) {  // if not tileable reverse x scale
			newGround.localScale = new Vector3 (newGround.localScale.x * -1, newGround.localScale.y, newGround.localScale.z);
		}

		newGround.parent = myTransform.parent;

		if (direction > 0) {
			newGround.GetComponent<Tiling> ().hasLeft = true;
			hasRight = true;
		} else {
			newGround.GetComponent<Tiling> ().hasRight = true;
			hasLeft = true;
		}
	}
}