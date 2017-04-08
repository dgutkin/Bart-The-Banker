using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class MenuBehaviour : MonoBehaviour {

	public GameStates stateManager = null;
	public GameObject menuItem; 
	public AudioSource audio;

	private bool _startActivated = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (_startActivated) {
			//start game and change game state to game mode
			stateManager.presentHowToPlay();
		}

	}

	void OnMouseDown() {
		
		switch (menuItem.name) {
		case "PlayButton":
			_startActivated = true;
			break;
		case "HighScoresButton":
			SceneManager.LoadScene("HighScores");
			break;
		case "BackButton":
			SceneManager.LoadScene ("Menu");
			break;
		case "MenuButton":
			SceneManager.LoadScene ("Menu");
			break;
		case "RetryButton":
			ShowAd ();
			SceneManager.LoadScene ("BartScene");
			break;
		case "CreditsButton":
			SceneManager.LoadScene ("Credits");
			break;
		}
	}

	public void ShowAd() {

		int randomNumber = Random.Range (1, 4);

		// Make the ad appear typically one in three times
		if (Advertisement.IsReady() && randomNumber == 1) {
			Advertisement.Show();
		}

	}

}
