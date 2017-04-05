using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class LeaderboardsBehaviour : MonoBehaviour {
	
	public Text leaderboardsText; 

	// Use this for initialization
	void Start () {
		//Load scores
		string[] leaderboards = PlayerPrefs.GetString("leaderboards").Split(';');
		string space = "\t\t\t";
		string text = "";
		for (int i = 0; i < leaderboards.Length; ++i) {
			if (i % 2 == 1) {
				text += space + "$" + leaderboards [i] + "\n";
			} else {
				text += leaderboards [i];
			}
		}
		leaderboardsText.text = text;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
