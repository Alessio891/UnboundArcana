using UnboundArcana.Core.Entities.Statuses;
using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Principles
{
	public enum ElementalPrincipleType
	{
		Acid,
		Air,
		Earth,
		Lightning,
		Water
	}

	[CreateAssetMenu(menuName = "Spells/Modules/Elemental Principle")]
	public class ElementalPrincipleDefinition : SpellModuleDefinition
	{
		public ElementalPrincipleType principle;
		public float damage;
		public float force = 2f;
		public float staggerDuration = 0.25f;
		public float arcRange = 0.9f;
		public float arcDamageMultiplier = 0.5f;
		public StatusDefinition status;
		public Sprite projectileSprite;
		public RuntimeAnimatorController projectileController;
		public Sprite beamSprite;
		public RuntimeAnimatorController beamController;
		public Sprite auraSprite;
		public RuntimeAnimatorController auraController;
		public Vector2 auraVisualOffset;
		public Color fallbackColor = Color.white;

		public override SpellModule CreateRuntime()
		{
			return new ElementalPrincipleModule(this);
		}
	}
}
