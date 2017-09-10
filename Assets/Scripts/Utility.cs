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

	public static bool buttonPressed = false;

	public static IEnumerator ButtonPress(Renderer buttonRenderer, AudioSource audioSource) {

		buttonPressed = true;

		// darken the sprite
		buttonRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 1f);

		audioSource.Play();

		// Only the first part of the clip is the required sound
		yield return new WaitForSeconds(audioSource.clip.length * 0.3f);

		// set the sprite back to normal
		buttonRenderer.material.color = new Color(1f, 1f, 1f, 1f);

		buttonPressed = false;

	}
		
}
