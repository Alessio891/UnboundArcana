using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
	private const string MainMenuScene = "MainMenu";

	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private float fadeInDuration = 0.8f;
	[SerializeField] private float holdDuration = 1.4f;
	[SerializeField] private float fadeOutDuration = 0.8f;

	private bool transitionStarted;

	private IEnumerator Start()
	{
		if (canvasGroup == null)
		{
			Debug.LogError("Splash screen requires a CanvasGroup.", this);
			yield return LoadMainMenu();
			yield break;
		}

		canvasGroup.alpha = 0f;
		yield return FadeTo(1f, fadeInDuration);
		yield return new WaitForSecondsRealtime(holdDuration);
		yield return FadeTo(0f, fadeOutDuration);
		yield return LoadMainMenu();
	}

	private IEnumerator FadeTo(float targetAlpha, float duration)
	{
		float startAlpha = canvasGroup.alpha;
		float elapsed = 0f;

		while (elapsed < duration)
		{
			elapsed += Time.unscaledDeltaTime;
			canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, duration <= 0f ? 1f : elapsed / duration);
			yield return null;
		}

		canvasGroup.alpha = targetAlpha;
	}

	private IEnumerator LoadMainMenu()
	{
		if (transitionStarted)
			yield break;

		transitionStarted = true;
		yield return SceneManager.LoadSceneAsync(MainMenuScene, LoadSceneMode.Additive);
		yield return SceneManager.UnloadSceneAsync(gameObject.scene);
	}
}
