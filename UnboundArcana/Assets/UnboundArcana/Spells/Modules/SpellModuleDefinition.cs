using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules
{
	public abstract class SpellModuleDefinition : ScriptableObject
	{
		public abstract SpellModule CreateRuntime();
	}
}