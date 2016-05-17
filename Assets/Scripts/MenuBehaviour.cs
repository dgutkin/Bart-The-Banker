using UnityEngine;
using System.Collections;

public class MenuBehaviour : MonoBehaviour {

	bool startActivated = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (startActivated) {
			//start game and change game state to game mode
		}
	}

	void onMouseDown() {
		startActivated = true;
	}
}
