using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Core.Events
{
	public class SpellConfigurationChangedEvent : SpellEvent
	{
		public SpellConfiguration Configuration { get; }
		public SpellConfigurationSlot Slot { get; }
		public SpellModuleDefinition Module { get; }
		public SpellModuleDefinition PreviousModule { get; }
		public SpellBehaviorDefinition Behavior { get; }

		public SpellConfigurationChangedEvent(
			SpellConfiguration configuration,
			SpellConfigurationSlot slot,
			SpellModuleDefinition module,
			SpellModuleDefinition previousModule)
		{
			Configuration = configuration;
			Slot = slot;
			Module = module;
			PreviousModule = previousModule;
		}

		public SpellConfigurationChangedEvent(
			SpellConfiguration configuration,
			SpellBehaviorDefinition behavior)
		{
			Configuration = configuration;
			Slot = SpellConfigurationSlot.Behavior;
			Behavior = behavior;
		}
	}
}
