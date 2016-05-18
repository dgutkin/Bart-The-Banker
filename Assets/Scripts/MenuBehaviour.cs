using UnityEngine;
using System.Collections;

public class MenuBehaviour : MonoBehaviour {

	bool startActivated = false;
	public GameStates stateManager = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (startActivated) {
			Debug.Log("activated");
			//start game and change game state to game mode
			stateManager.startGame();
		}
	}

	void OnMouseDown() {
		Debug.Log("mouse_down");
		startActivated = true;
	}
}
