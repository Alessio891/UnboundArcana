using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Spells.Services
{
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

	public class ModuleRewardService
	{
		private readonly ModuleRewardTable rewardTable;

		public ModuleRewardService(
			ModuleRewardTable rewardTable)
		{
			this.rewardTable = rewardTable;
		}

		public List<SpellModuleDefinition> RollModules(
			IReadOnlyList<SpellModuleDefinition> availableModules,
			int count)
		{
			List<SpellModuleDefinition> remaining =
				new(availableModules);

			List<SpellModuleDefinition> result = new();

			while (remaining.Count > 0 &&
				result.Count < count)
			{
				ModuleRarity rarity =
					RollRarity(remaining);

				List<SpellModuleDefinition> candidates =
					remaining
						.Where(x => x.Rarity == rarity)
						.ToList();

				if (candidates.Count == 0)
				{
					continue;
				}

				SpellModuleDefinition selected =
					candidates[
						Random.Range(
							0,
							candidates.Count)];

				result.Add(selected);
				remaining.Remove(selected);
			}

			return result;
		}

		private ModuleRarity RollRarity(
			List<SpellModuleDefinition> modules)
		{
			Dictionary<ModuleRarity, int> weights =
				new();

			int totalWeight = 0;

			foreach (ModuleRarity rarity in System.Enum.GetValues(typeof(ModuleRarity)))
			{
				if (!modules.Exists(x => x.Rarity == rarity))
				{
					continue;
				}

				int weight =
					rewardTable.GetWeight(rarity);

				weights.Add(
					rarity,
					weight
				);

				totalWeight += weight;
			}

			int roll =
				Random.Range(0, totalWeight);

			foreach (var pair in weights)
			{
				if (roll < pair.Value)
				{
					return pair.Key;
				}

				roll -= pair.Value;
			}

			return ModuleRarity.Common;
		}
	}
}