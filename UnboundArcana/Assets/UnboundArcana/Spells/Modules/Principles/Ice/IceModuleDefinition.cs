using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Core.Entities.Statuses;
using UnityEngine.Serialization;

namespace UnboundArcana.Spells.Modules.Ice
{
	[CreateAssetMenu(menuName = "Spells/Modules/Ice")]
	public class IceModuleDefinition : SpellModuleDefinition
	{
		public float damage = 1.0f;
		public Sprite projectileSprite;
		public ChilledStatusDefinition chilledStatus;
		[FormerlySerializedAs("contorller")]
		public RuntimeAnimatorController controller;
		public Sprite beamSprite;
		public RuntimeAnimatorController beamController;
		public Sprite auraSprite;
		public RuntimeAnimatorController auraController;
		public override SpellModule CreateRuntime()
		{
			return new IceModule(this);
		}
	}
}
