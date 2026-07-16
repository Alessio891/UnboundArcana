using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Number")]
public class NumberSetting : SettingDefinition
{
	public float defaultValue;
	public float min;
	public float max;

	public override object GetDefaultValue()
	{
		return defaultValue;
	}
}