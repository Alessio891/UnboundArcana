using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules
{
	public enum SpellModuleType {
		Principle,
		Catalyst,
		Flux
	}
	public abstract class SpellModuleDefinition : ScriptableObject
	{
		[SerializeField]
		private SpellModuleType category = SpellModuleType.Catalyst;
		[SerializeField]
		private Sprite icon;

		public Sprite Icon => icon;

		public SpellModuleType Type => category;

		public abstract SpellModule CreateRuntime();
	}
}