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

	public static IEnumerator FadeSpriteOut(SpriteRenderer renderer, float showDuration, float fadeDuration) {
		
		yield return new WaitForSeconds (showDuration);
		renderer.enabled = false;

	}

	public static IEnumerator FadeLineOut(LineRenderer renderer, float showDuration, float fadeDuration) {

		yield return new WaitForSeconds (showDuration);
		renderer.enabled = false;

	}
		
}
