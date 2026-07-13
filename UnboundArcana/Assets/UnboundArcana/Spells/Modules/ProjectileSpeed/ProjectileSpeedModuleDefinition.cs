using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.ProjectileSpeed
{
	[CreateAssetMenu(menuName = "Spells/Modules/Projectile Speed")]
	public class ProjectileSpeedModuleDefinition : SpellModuleDefinition
	{
		public float acceleration = 5f;

		public override SpellModule CreateRuntime()
		{
			return new ProjectileSpeedModule(this);
		}
	}
}