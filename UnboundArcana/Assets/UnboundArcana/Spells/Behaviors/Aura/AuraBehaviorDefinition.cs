using UnityEngine;
using UnityEngine.Serialization;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Behaviors.Aura
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Behaviors/Aura")]
	public class AuraBehaviorDefinition : SpellBehaviorDefinition
	{
		public GameObject auraPrefab;
		public float duration = 5f;
		[FormerlySerializedAs("size")]
		[Tooltip("Base radius in world units. One dungeon tile is 0.3 units.")]
		public float radius = 0.9f;
		public bool followOwner = true;
		public override SpellBehavior CreateRuntime()
		{
			AuraBehavior behavior = new();
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
				StatKeys.Spell.Duration,
				duration,
				this
			);

			stats.AddBase(StatKeys.Spell.Size, 1f, this);
		}
	}
}
