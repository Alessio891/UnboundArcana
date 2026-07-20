using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
	private void Start()
	{
#if UNITY_EDITOR
		var scene = UnityEditor.EditorPrefs.GetString(
			"Bootstrap.TargetScene",
			string.Empty
		);

		if (!string.IsNullOrEmpty(scene))
		{
			SceneManager.LoadScene(scene, LoadSceneMode.Additive);
		}
#endif
	}
}