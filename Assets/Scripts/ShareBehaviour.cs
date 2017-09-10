using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ShareBehaviour : MonoBehaviour {

	private Renderer _shareButtonRenderer;
	private AudioSource _audioClip;

	// Use this for initialization
	void Start () {

		_shareButtonRenderer = gameObject.GetComponent<Renderer> ();
		_audioClip = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown() {

		StartCoroutine ("TakeScreenshot");

	}

	IEnumerator TakeScreenshot() {

		StartCoroutine (Utility.ButtonPress(_shareButtonRenderer, _audioClip));

		while (Utility.buttonPressed) {
			yield return new WaitForSeconds (0.1f);
		}

		yield return new WaitForEndOfFrame();

		int width = Screen.width;
		int height = Screen.height;
		Texture2D texture = new Texture2D (width, height, TextureFormat.RGB24, false);

		texture.ReadPixels (new Rect (0, 0, width, height), 0, 0);
		texture.Apply ();
		byte[] screenshot = texture.EncodeToPNG ();
		Destroy (texture);

		ShareScreenshot (screenshot);
			
	}

	private void ShareScreenshot(byte[] screenshot) {
		
		#if UNITY_ANDROID

			string path = Application.persistentDataPath + "/HighScoreScreenshot.png";
			File.WriteAllBytes (path, screenshot);
			
			AndroidJavaClass intentClass = new AndroidJavaClass ("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject ("android.content.Intent");
			
			intentObject.Call<AndroidJavaObject> ("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			intentObject.Call<AndroidJavaObject> ("setType", "image/*");
			intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_SUBJECT"), "Bart the Banker Highscore");
			intentObject.Call<AndroidJavaObject> ("putExtra", intentClass.GetStatic<string> ("EXTRA_TITLE"), "Bart the Banker Highscore");
			
			AndroidJavaClass uriClass = new AndroidJavaClass ("android.net.Uri");
			AndroidJavaClass fileClass = new AndroidJavaClass ("java.io.File");
			
			AndroidJavaObject fileObject = new AndroidJavaObject ("java.io.File", path);
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject> ("fromFile", fileObject);

			bool fileExist = fileObject.Call<bool> ("exists");

			if (fileExist) {
			
				intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
				AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
				currentActivity.Call("startActivity", intentObject);

			}

		#elif UNITY_IOS

			

		#endif

	}

	private void OnHideUnity(bool isGameShown) {

		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

	}
}
