using UnityEngine;
using UnityEngine.SceneManagement;

public enum MainMenuState {
	Main,
	NewResearch,
	ResumeResearch,
	Settings
}

public class MainMenuStateController : MonoBehaviour
{
	public MainMenuState CurrentState = MainMenuState.Main;

	[SerializeField] private SettingsMenuController SettingsMenu;
	[SerializeField] private NewResearchController NewReserachMenu;

	private void Awake()
	{
		if (SettingsManager.Instance == null) {
			SceneManager.LoadScene("Bootstrap", LoadSceneMode.Additive);
		}
	}

	public void ChangeMenuState(int newState) {
		CurrentState = (MainMenuState)newState;

		SettingsMenu.gameObject.SetActive(false);
		NewReserachMenu.gameObject.SetActive(false);

		if (CurrentState == MainMenuState.Settings) {
			SettingsMenu.gameObject.SetActive(true);
		} else if (CurrentState == MainMenuState.Main) {

		} else if (CurrentState == MainMenuState.NewResearch) {
			NewReserachMenu.gameObject.SetActive(true);
		}
		else {
			SettingsMenu.gameObject.SetActive(false);
		}
	}
	public void ApplySettings() {
		SettingsManager.Instance.Save();
	}
}
