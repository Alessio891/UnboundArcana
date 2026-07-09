using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Behaviors.Projectile
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Behaviors/Projectile")]
	public class ProjectileBehaviorDefinition : SpellBehaviorDefinition
	{
		public GameObject projectilePrefab;

		public override SpellBehavior CreateRuntime()
		{
			ProjectileBehavior behavior = new();
			behavior.InitializeDefinition(this);

			return behavior;
		}
	}
}