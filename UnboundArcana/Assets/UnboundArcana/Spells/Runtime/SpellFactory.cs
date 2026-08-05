using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public static class SpellFactory
	{
		public static bool TryCreate(
			SpellConfiguration configuration,
			SpellRuntimeContext context,
			GameObject owner,
			out SpellInstance instance,
			out SpellConfigurationValidationResult validation)
		{
			instance = null;
			validation = configuration == null
				? new SpellConfigurationValidationResult(SpellConfigurationValidationError.MissingBehavior, "Spell configuration is null.")
				: configuration.Validate();

			if (!validation.IsValid)
			{
				return false;
			}

			instance = new SpellInstance(context, owner);
			instance.behavior = configuration.Behavior.CreateRuntime();

			configuration.Behavior.ApplyStats(
				instance.Stats
			);

			foreach (SpellModuleDefinition moduleDefinition in configuration.Modules)
			{
				instance.modules.Add(
					moduleDefinition.CreateRuntime()
				);
			}

			foreach (SpellModule module in instance.modules)
			{
				module.ApplyStats(instance.Stats);
			}

			instance.Initialize();
			return true;
		}

		public static SpellInstance Create(
			SpellConfiguration configuration,
			SpellRuntimeContext context,
			GameObject owner)
		{
			TryCreate(configuration, context, owner, out SpellInstance instance, out _);
			return instance;
		}
	}
}
