using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules
{
	public abstract class SpellModuleDefinition : ScriptableObject
	{
		[SerializeField]
		private ModuleCategory category = ModuleCategory.Other;

		public ModuleCategory Category => category;

		public abstract SpellModule CreateRuntime();
	}
}