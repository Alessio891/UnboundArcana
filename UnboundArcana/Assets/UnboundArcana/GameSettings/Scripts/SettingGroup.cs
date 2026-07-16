using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Group")]
public class SettingGroup : ScriptableObject
{
	public string groupName;

	public List<SettingDefinition> settings;
}