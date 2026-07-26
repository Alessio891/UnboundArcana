
using UnityEngine;

[CreateAssetMenu(
		menuName = "Unbound Arcana/Module Reward Table"
	)]
public class ModuleRewardTable : ScriptableObject
{
	public int commonWeight = 60;
	public int uncommonWeight = 25;
	public int rareWeight = 10;
	public int uniqueWeight = 5;

	public int GetWeight(ModuleRarity rarity)
	{
		return rarity switch
		{
			ModuleRarity.Common => commonWeight,
			ModuleRarity.Uncommon => uncommonWeight,
			ModuleRarity.Rare => rareWeight,
			ModuleRarity.Unique => uniqueWeight,
			_ => 0
		};
	}
}