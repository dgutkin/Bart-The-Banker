using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuBehaviour : MonoBehaviour {

	public GameStates stateManager = null;
	public GameObject menuItem; 
	private bool _startActivated = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (_startActivated) {
			Debug.Log("activated");
			//start game and change game state to game mode
			stateManager.startGame();
		}
	}

	void OnMouseDown() {
		Debug.Log ("mouse_down");
		switch (menuItem.name) {
		case "PlayButton":
			_startActivated = true;
			break;
		case "LeaderboardsButton":
			Debug.Log ("leaderboardsbutton");
			SceneManager.LoadScene("Leaderboards");
			break;
		case "BackButton":
			SceneManager.LoadScene ("Menu");
			break;
		case "MenuButton":
			SceneManager.LoadScene ("Menu");
			break;
		case "RetryButton":
			//_startActivated = true;
			SceneManager.LoadScene ("BartScene");
			break;
		}
	}
}
