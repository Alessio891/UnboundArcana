using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Spells.Modules.Ice
{
	[CreateAssetMenu(menuName = "Spells/Modules/Ice")]
	public class IceModuleDefinition : SpellModuleDefinition
	{
		public float damage = 1.0f;
		public Sprite projectileSprite;
		public ChilledStatusDefinition chilledStatus;
		public override SpellModule CreateRuntime()
		{
			return new IceModule(this);
		}
	}
}