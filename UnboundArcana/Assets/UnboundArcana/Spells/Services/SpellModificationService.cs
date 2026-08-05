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
			SpellConfiguration configuration,
			SpellBehaviorDefinition behavior
		)
		{
			if (configuration == null || behavior == null || configuration.Behavior == behavior) { return false; }

			foreach (SpellModuleDefinition module in configuration.Modules)
			{
				if (!module.SupportsBehavior(behavior)) { return false; }
			}

			configuration.SetBehavior(behavior);
			events.Publish(new SpellConfigurationChangedEvent(configuration, behavior));
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

			configuration.TryGetAvailableSlot(module.Type, out SpellConfigurationSlot slot);
			return InstallModule(configuration, slot, module);
		}

		public bool CanAddModule(
			SpellConfiguration configuration,
			SpellModuleDefinition module)
		{
			return configuration != null &&
				module != null &&
				!configuration.HasModule(module) &&
				module.CanAddTo(configuration) &&
				configuration.TryGetAvailableSlot(module.Type, out _);
		}

		public bool TryReplaceModule(
			SpellConfiguration configuration,
			SpellConfigurationSlot slot,
			SpellModuleDefinition module)
		{
			if (!CanReplaceModule(configuration, slot, module))
			{
				return false;
			}

			SpellModuleDefinition previousModule = configuration.GetModule(slot);
			configuration.SetModule(slot, module);
			events.Publish(new SpellConfigurationChangedEvent(configuration, slot, module, previousModule));
			return true;
		}

		public bool CanReplaceModule(
			SpellConfiguration configuration,
			SpellConfigurationSlot slot,
			SpellModuleDefinition module)
		{
			if (configuration == null || module == null || slot == SpellConfigurationSlot.Behavior)
			{
				return false;
			}

			if (!MatchesSlot(module.Type, slot) || configuration.GetModule(slot) == null || configuration.GetModule(slot) == module)
			{
				return false;
			}

			if (configuration.HasModule(module))
			{
				return false;
			}

			if (!module.CanAddTo(configuration))
			{
				return false;
			}

			SpellModuleDefinition previousModule = configuration.GetModule(slot);
			configuration.SetModule(slot, module);
			bool compatible = true;
			foreach (SpellModuleDefinition installedModule in configuration.Modules)
			{
				if (!installedModule.CanAddTo(configuration))
				{
					compatible = false;
					break;
				}
			}

			configuration.SetModule(slot, previousModule);
			return compatible;
		}

		public bool TryRemoveModule(
			SpellConfiguration configuration,
			SpellModuleDefinition module)
		{
			if (configuration == null || module == null || !configuration.TryGetSlot(module, out SpellConfigurationSlot slot))
			{
				return false;
			}

			return TryRemoveModule(configuration, slot);
		}

		public bool TryRemoveModule(
			SpellConfiguration configuration,
			SpellConfigurationSlot slot)
		{
			if (configuration == null || slot == SpellConfigurationSlot.Behavior || configuration.GetModule(slot) == null)
			{
				return false;
			}

			SpellModuleDefinition module = configuration.GetModule(slot);
			configuration.SetModule(slot, null);
			events.Publish(new SpellConfigurationChangedEvent(configuration, slot, null, module));
			return true;
		}

		private bool InstallModule(
			SpellConfiguration configuration,
			SpellConfigurationSlot slot,
			SpellModuleDefinition module)
		{
			configuration.SetModule(slot, module);
			events.Publish(new SpellConfigurationChangedEvent(configuration, slot, module, null));
			return true;
		}

		private bool MatchesSlot(
			SpellModuleType type,
			SpellConfigurationSlot slot)
		{
			return (type == SpellModuleType.Principle && slot == SpellConfigurationSlot.Principle) ||
				(type == SpellModuleType.Catalyst && (slot == SpellConfigurationSlot.CatalystA || slot == SpellConfigurationSlot.CatalystB)) ||
				(type == SpellModuleType.Flux && slot == SpellConfigurationSlot.Flux);

		}
	}
}
