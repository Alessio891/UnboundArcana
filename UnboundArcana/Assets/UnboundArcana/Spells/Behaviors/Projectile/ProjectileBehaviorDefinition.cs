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
		public float baseDamage = 1.0f;
		[Tooltip("Visual diameter and collision diameter in world units. One dungeon tile is 0.3 units.")]
		public float diameter = 0.3f;

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
				StatKeys.Spell.CastTime,
				castTime,
				this
			);

			stats.AddBase(
				StatKeys.Spell.Speed,
				speed,
				this
			);

			stats.AddBase(
				StatKeys.Spell.Duration,
				lifetime,
				this
			);

			stats.AddBase(
				StatKeys.Spell.Damage,
				baseDamage,
				this
			);

			stats.AddBase(StatKeys.Spell.Size, 1.0f, this);
		}
	}

}
