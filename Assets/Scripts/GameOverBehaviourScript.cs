using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GameOverBehaviourScript : MonoBehaviour {

	public Text newHighscore;
	public Text totalCash;
	public Text totalTax;
	public Text totalScore;
	public Text highscore;

	// Use this for initialization
	void Start () {
		string dollar = "$";
		int score = PlayerPrefs.GetInt ("score");
		int cash = PlayerPrefs.GetInt ("cash");
		int tax = PlayerPrefs.GetInt ("tax");

		string[] leaderboards = PlayerPrefs.GetString ("leaderboards").Split(';');
		if (Int32.Parse(leaderboards [1]) > score) {
			newHighscore.text = "";
		}
		highscore.text = dollar + leaderboards [1];

		totalCash.text = "+" + dollar + cash.ToString ();
		totalTax.text = "-" + dollar + tax.ToString ();
		totalScore.text = dollar + score.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
