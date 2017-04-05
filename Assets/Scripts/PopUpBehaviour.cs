using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpBehaviour : MonoBehaviour {

	private Text _text;

	// Use this for initialization
	void Start () {
		
		_text = GetComponent<Text> ();
		_text.CrossFadeAlpha (0f, 1f, false);

	}
	
	// Update is called once per frame
	void Update () {

	}
}
