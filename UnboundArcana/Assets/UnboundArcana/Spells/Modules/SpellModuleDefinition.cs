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
		private ModuleRarity rarity = ModuleRarity.Common;
		public ModuleRarity Rarity => rarity;
		[SerializeField]
		private Sprite icon;
		[SerializeField]
		private string moduleName;
		[SerializeField]
		[Multiline(5)]
		private string moduleDescription;

		[SerializeField]
		[Range(0.0f, 100.0f)]
		private float complexity = 1.0f;
		[SerializeField]
		[Range(0.0f, 100.0f)]
		private float instability = 1.0f;

		public Sprite Icon => icon;

		public SpellModuleType Type => category;

		public abstract SpellModule CreateRuntime();
	}
}