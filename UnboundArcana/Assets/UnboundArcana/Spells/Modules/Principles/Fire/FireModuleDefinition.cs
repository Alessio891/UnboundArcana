using UnboundArcana.Core.Entities.Statuses;
using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Fire
{
	[CreateAssetMenu(menuName = "Spells/Modules/Fire")]
	public class FireModuleDefinition : SpellModuleDefinition
	{
		public float damage;
		public BurningStatusDefinition burningStatus;
		public Sprite projectileSprite;
		public RuntimeAnimatorController controller;
		public Sprite beamSprite;
		public RuntimeAnimatorController beamController;
		public Sprite auraSprite;
		public RuntimeAnimatorController auraController;
		public override SpellModule CreateRuntime()
		{
			return new FireModule(this);
		}
	}
}
