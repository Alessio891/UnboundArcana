using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Core.Events
{
	public class RewardSelectedEvent : SpellEvent
	{
		public SpellModuleDefinition Reward { get; }

		public RewardSelectedEvent(SpellModuleDefinition reward)
		{
			Reward = reward;
		}
	}
}