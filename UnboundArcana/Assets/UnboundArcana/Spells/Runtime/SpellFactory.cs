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
			GameEventBus gameEventBus,
			GameObject owner)
		{
			SpellInstance instance = new(gameEventBus, owner);

			instance.behavior = definition.behavior.CreateRuntime();

			if (definition.modules != null)
			{
				foreach (SpellModuleDefinition moduleDefinition in definition.modules)
				{
					instance.modules.Add(
						moduleDefinition.CreateRuntime()
					);
				}
			}

			instance.Initialize();

			return instance;
		}
	}
}