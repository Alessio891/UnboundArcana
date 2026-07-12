using System.Collections.Generic;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Core.Events
{
	public class RewardOfferedEvent : SpellEvent
	{
		public IReadOnlyList<SpellModuleDefinition> Rewards { get; }

		public RewardOfferedEvent(
			IReadOnlyList<SpellModuleDefinition> rewards)
		{
			Rewards = rewards;
		}
	}
}