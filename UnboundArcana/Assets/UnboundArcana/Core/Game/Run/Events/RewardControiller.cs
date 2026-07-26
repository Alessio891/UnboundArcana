using System.Collections.Generic;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Services;
using UnityEngine;

namespace UnboundArcana.Core.Expedition
{
	public class RewardController
	{
		private readonly ModuleRewardService rewardService;

		public RewardController(
			ModuleRewardService rewardService)
		{
			this.rewardService = rewardService;
		}

		public List<SpellModuleDefinition> GenerateModuleRewards(
			IReadOnlyList<SpellModuleDefinition> availableModules,
			int count)
		{
			if (availableModules == null ||
				availableModules.Count == 0)
			{
				return new List<SpellModuleDefinition>();
			}

			return rewardService.RollModules(
				availableModules,
				count);
		}
	}
}