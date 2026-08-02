using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
	private const string SplashScene = "Splash";

	private void Start()
	{
		var scene = SplashScene;

#if UNITY_EDITOR
		var editorScene = UnityEditor.EditorPrefs.GetString(
			"Bootstrap.TargetScene",
			string.Empty
		);

		if (!string.IsNullOrEmpty(editorScene))
		{
			scene = editorScene;
		}
#endif

		SceneManager.LoadScene(scene, LoadSceneMode.Additive);
	}
}
