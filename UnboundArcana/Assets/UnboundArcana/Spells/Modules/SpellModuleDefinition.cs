using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Stats;
using UnityEngine;

namespace UnboundArcana.Spells.Modules
{
	public abstract class SpellModuleDefinition : ScriptableObject
	{
		public abstract SpellModule CreateRuntime();

	}
}