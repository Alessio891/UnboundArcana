using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Behaviors.Beam
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Behaviors/Beam")]
	public class BeamBehaviorDefinition : SpellBehaviorDefinition
	{
		public GameObject beamPrefab;
		[Tooltip("Range in world units. One dungeon tile is 0.3 units.")]
		public float range = 1.8f;
		[Tooltip("Width in world units. One dungeon tile is 0.3 units.")]
		public float width = 0.3f;
		public float damage = 2f;
		[Tooltip("Delay before the beam can deal its first damage tick.")]
		public float startupDelay = 0.1f;
		public float damageInterval = 0.2f;

		public override SpellBehavior CreateRuntime()
		{
			BeamBehavior behavior = new();
			behavior.InitializeDefinition(this);

			return behavior;
		}

		public override void ApplyStats(StatCollection stats)
		{
			stats.AddBase(StatKeys.Spell.CastTime, castTime, this);
			stats.AddBase(StatKeys.Spell.Damage, damage, this);
			stats.AddBase(StatKeys.Spell.Size, 1f, this);
		}
	}
}
