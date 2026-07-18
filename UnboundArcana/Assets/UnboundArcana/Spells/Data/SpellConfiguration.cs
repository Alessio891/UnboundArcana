using System.Collections.Generic;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Spells.Data
{
	public class SpellConfiguration
	{
		public SpellBehaviorDefinition behavior;
		public List<SpellModuleDefinition> modules = new();

		public SpellConfiguration(
			SpellDefinition definition)
		{
			behavior = definition.behavior;

			if (definition.modules != null)
			{
				modules.AddRange(definition.modules);
			}
		}

		public void SetBehavior(SpellBehaviorDefinition behavior) {
			this.behavior = behavior;
		}

		public void AddModule(
			SpellModuleDefinition module)
		{
			if (module == null)
			{
				return;
			}

			modules.Add(module);
		}

		public void RemoveModule(
			SpellModuleDefinition module)
		{
			if (module == null)
			{
				return;
			}

			modules.Remove(module);
		}

		public bool HasModule(
			SpellModuleDefinition module)
		{
			return modules.Contains(module);
		}
	}
}