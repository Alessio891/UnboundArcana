using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Behaviors.Projectile
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Behaviors/Projectile")]
	public class ProjectileBehaviorDefinition : SpellBehaviorDefinition
	{
		public GameObject projectilePrefab;
		public float lifetime = 1.0f;
		public float speed = 1.0f;

		public override SpellBehavior CreateRuntime()
		{
			ProjectileBehavior behavior = new();
			behavior.InitializeDefinition(this);
			

			return behavior;
		}


		public override void ApplyStats(
			StatCollection stats)
		{
			stats.AddBase(
				StatId.Speed,
				speed,
				this
			);

			stats.AddBase(
				StatId.Duration,
				lifetime,
				this
			);

			stats.AddBase(StatId.Size, 1.0f, this);
		}
	}

}