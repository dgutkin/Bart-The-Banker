using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;


public class MenuBehaviour : MonoBehaviour {

	public GameStates stateManager = null;
	public GameObject menuItem;
	public AudioSource audioSource;

	private bool _startActivated = false;
	private Renderer menuItemRenderer;

	// Use this for initialization
	void Start () {
		
		audioSource = GetComponent<AudioSource> ();
		menuItemRenderer = menuItem.GetComponent<Renderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		
		if (_startActivated) {
			//start game and change game state to game mode
			stateManager.startGame();
		}

		// Update volume
		audioSource.volume = PlayerPrefs.GetFloat(Constants.SOUND_VOLUME, Constants.DEFAULT_SOUND_VOLUME);

	}

	void OnMouseDown() {
		
		if (menuItemRenderer.enabled) {
			
			StartCoroutine ("ActivateButton");

		}

	}

	IEnumerator ActivateButton() {

		audioSource.Play();

		// Only the first part of the clip is the required sound
		yield return new WaitForSeconds(audioSource.clip.length * 0.5f);

		switch (menuItem.name) {
		case "PlayButton":
			ShowAd ();
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
		case "SettingsButton":
			SceneManager.LoadScene ("Settings");
			break;
		case "HowToPlayButton":
			SceneManager.LoadScene ("HowToPlay");
			break;
		case "QuitButton":
			SceneManager.LoadScene ("Menu");
			break;
		case "ResumeButton":
			// action handled in pause behaviour
			// placeholder to play the button tap
			break;
		}

	}

	public void ShowAd() {

		if (PlayerPrefs.HasKey ("leaderboards")) {
			
			List<string> leaderboards = new List<string> (PlayerPrefs.GetString ("leaderboards").Split (';'));
			if (leaderboards.Count > 6) {
				// The ad only appears once the game is played more than three times
				int randomNumber = Random.Range (1, 4);

				// Make the ad appear one in three times on average
				if (Advertisement.IsReady () && randomNumber == 1) {
					Advertisement.Show ();
				}
			
			}

		}

	}

}
