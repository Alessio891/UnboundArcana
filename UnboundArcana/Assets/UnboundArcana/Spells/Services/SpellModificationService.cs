using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Spells.Services
{
	public class SpellModificationService
	{
		private readonly GameEventBus events;

		public SpellModificationService(
			GameEventBus events)
		{
			this.events = events;
		}

		public bool TrySetBehavior(
			SpellConfiguration configuraiton,
			SpellBehaviorDefinition behavior
		) {
			configuraiton.SetBehavior( behavior );
			return true;
		}

		public bool TryAddModule(
			SpellConfiguration configuration,
			SpellModuleDefinition module)
		{
			if (!CanAddModule(configuration, module))
			{
				return false;
			}

			configuration.AddModule(module);

			events.Publish(
				new SpellConfigurationChangedEvent(
					configuration,
					module
				)
			);

			return true;
		}

		public bool CanAddModule(
			SpellConfiguration configuration,
			SpellModuleDefinition module)
		{
			return configuration != null &&
				module != null &&
				!configuration.HasModule(module);
		}

		public bool TryRemoveModule(
			SpellConfiguration configuration,
			SpellModuleDefinition module)
		{
			if (configuration == null ||
				module == null ||
				!configuration.HasModule(module))
			{
				return false;
			}

			configuration.RemoveModule(module);

			events.Publish(
				new SpellConfigurationChangedEvent(
					configuration,
					module
				)
			);

			return true;
		}
	}
}