using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public static class SpellFactory
	{
		public static SpellInstance Create(
			SpellConfiguration configuration,
			SpellRuntimeContext context,
			GameObject owner)
		{
			SpellInstance instance = new(context, owner);

			instance.behavior = configuration.behavior.CreateRuntime();

			configuration.behavior.ApplyStats(
				instance.Stats
			);

			foreach (SpellModuleDefinition moduleDefinition in configuration.modules)
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

			return instance;
		}
	}
}