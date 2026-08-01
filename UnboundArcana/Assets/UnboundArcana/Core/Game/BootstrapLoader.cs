using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
	private const string MainMenuScene = "MainMenu";

	private void Start()
	{
		var scene = MainMenuScene;

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
