using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public static class SpellFactory
	{

		public static SpellInstance Create(
			SpellDefinition definition,
			SpellRuntimeContext context,
			GameObject owner)
		{
			SpellInstance instance = new(context, owner);

			instance.behavior = definition.behavior.CreateRuntime();

			definition.behavior.ApplyStats(
				instance.Stats
			);

			if (definition.modules != null)
			{
				foreach (SpellModuleDefinition moduleDefinition in definition.modules)
				{
					instance.modules.Add(
						moduleDefinition.CreateRuntime()
					);
				}
			}

			definition.behavior.ApplyStats(instance.Stats);

			foreach (SpellModule module in instance.modules)
			{
				module.ApplyStats(instance.Stats);
			}


			instance.Initialize();

			return instance;
		}
	}
}