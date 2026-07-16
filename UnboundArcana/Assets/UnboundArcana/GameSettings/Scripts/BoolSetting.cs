using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Bool")]
public class BoolSetting : SettingDefinition
{
	public bool defaultValue;

	public override object GetDefaultValue()
	{
		return defaultValue;
	}
}