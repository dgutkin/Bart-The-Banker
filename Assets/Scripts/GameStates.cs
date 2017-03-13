using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameStates : MonoBehaviour {

	public static bool gameActive = true;

	public enum displayStates {

		menuScreen = 0,
		howtoplayScreen,
		gameScreen

	}

	// Use this for initialization
	void Start () {
		
	}

	public void changeDisplayState(displayStates newState) {

		switch (newState) {
		case displayStates.menuScreen:
			gameActive = false;
			SceneManager.LoadScene ("Menu");
			break;
		case displayStates.howtoplayScreen:
			gameActive = false;
			SceneManager.LoadScene ("HowToPlay");
			break;
		case displayStates.gameScreen:
			gameActive = true;
			SceneManager.LoadScene ("BartScene");
			break;
		}

	}

	public void presentHowToPlay() {

		changeDisplayState (displayStates.howtoplayScreen);

	}

	public void startGame() {
		
		changeDisplayState (displayStates.gameScreen);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
