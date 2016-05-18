using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStates : MonoBehaviour {

	public static bool gameActive = true;

	public enum displayStates {

		menuScreen = 0,
		gameScreen

	}

	// Use this for initialization
	void Start () {
		
	}

	public void changeDisplayState(displayStates newState) {

		Debug.Log("change");
		switch (newState) {
		case displayStates.menuScreen:
			gameActive = false;
			SceneManager.LoadScene (0);
			break;
		case displayStates.gameScreen:
			gameActive = true;
			SceneManager.LoadScene (1);
			break;
		}

	}

	public void startGame() {
		Debug.Log("start");
		changeDisplayState (displayStates.gameScreen);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
