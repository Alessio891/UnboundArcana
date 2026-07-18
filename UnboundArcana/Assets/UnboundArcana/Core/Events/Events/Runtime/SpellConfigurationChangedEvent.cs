using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Core.Events
{
	public class SpellConfigurationChangedEvent : SpellEvent
	{
		public SpellConfiguration Configuration { get; }
		public SpellModuleDefinition Module { get; }

		public SpellConfigurationChangedEvent(
			SpellConfiguration configuration,
			SpellModuleDefinition module)
		{
			Configuration = configuration;
			Module = module;
		}
	}
}