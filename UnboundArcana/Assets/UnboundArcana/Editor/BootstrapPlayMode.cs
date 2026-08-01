using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class BootstrapEditorContext
{
	private const string TargetSceneKey = "Bootstrap.TargetScene";

	public static void SetTargetScene(string path)
	{
		EditorPrefs.SetString(TargetSceneKey, path);
	}

	public static string GetTargetScene()
	{
		return EditorPrefs.GetString(TargetSceneKey, string.Empty);
	}
}

[InitializeOnLoad]
public static class BootstrapPlayMode
{
	private const string BootstrapScene = "Assets/Scenes/Bootstrap.unity";
	private const string TargetSceneKey = "Bootstrap.TargetScene";

	static BootstrapPlayMode()
	{
		EditorApplication.playModeStateChanged += OnPlayModeChanged;
	}

	private static void OnPlayModeChanged(PlayModeStateChange state)
	{
		switch (state)
		{
			case PlayModeStateChange.ExitingEditMode:
				{
					var currentScene = SceneManager.GetActiveScene();

					if (currentScene.path == BootstrapScene)
					{
						EditorPrefs.DeleteKey(TargetSceneKey);
					}
					else
					{
						EditorPrefs.SetString(TargetSceneKey, currentScene.path);

						var bootstrap = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScene);
						EditorSceneManager.playModeStartScene = bootstrap;
					}

					break;
				}

			case PlayModeStateChange.EnteredEditMode:
				{
					EditorSceneManager.playModeStartScene = null;
					break;
				}
		}
	}
}
