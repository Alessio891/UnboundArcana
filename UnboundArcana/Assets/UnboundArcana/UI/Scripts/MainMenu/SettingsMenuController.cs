using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
	[SerializeField] private VerticalLayoutGroup ListContainer;
	[SerializeField] private GameSettingEntryController EntryPrefab;
	[SerializeField] private SettingGroup GroupToLoad;

	private void Start()
	{
		//SettingsManager.Instance.Initialize(new List<SettingGroup> { GroupToLoad });
		ShowSettingsGroup(GroupToLoad);
	}
	public void ShowSettingsGroup(SettingGroup group) {
		foreach (Transform child in ListContainer.transform)
		{
			Destroy(child.gameObject);
		}

		foreach(SettingDefinition setting in group.settings) {
			GameSettingEntryController entry = Instantiate(EntryPrefab);
			entry.LoadSetting(setting);
			entry.transform.SetParent(ListContainer.transform);
		}
	}
}
