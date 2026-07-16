using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Option")]
public class OptionSetting : SettingDefinition
{
	public string[] options;
	public int defaultValue;

	public override object GetDefaultValue()
	{
		return defaultValue;
	}
}