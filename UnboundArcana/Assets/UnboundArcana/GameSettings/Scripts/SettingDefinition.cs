using UnityEngine.Rendering;
using UnityEngine;

public abstract class SettingDefinition : ScriptableObject
{
	public string id;
	public string displayName;
	public SettingGroup group;

	public abstract object GetDefaultValue();
}