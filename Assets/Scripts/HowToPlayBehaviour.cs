using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayBehaviour : MonoBehaviour {

	public GameStates stateManager;

	private AudioSource _audioSource;

	// Use this for initialization
	void Start () {

		_audioSource = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {

		StartCoroutine ("StartGame");

	}

	IEnumerator StartGame() {

		_audioSource.Play ();

		yield return new WaitForSeconds (_audioSource.clip.length - 0.5f);

		stateManager.startGame ();

	}

}
