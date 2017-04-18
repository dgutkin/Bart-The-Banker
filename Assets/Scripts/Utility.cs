using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utility {

	public static IEnumerator FadeTextOut(Text text, float showDuration, float fadeDuration) {
		yield return new WaitForSeconds (showDuration);
		text.CrossFadeAlpha (0f, fadeDuration, false);
		yield return new WaitForSeconds (fadeDuration);
		text.enabled = false;
	}
		
}
