using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Behaviors.Aura
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Behaviors/Aura")]
	public class AuraBehaviorDefinition : SpellBehaviorDefinition
	{
		public GameObject auraPrefab;
		public float duration = 5f;
		public float size = 1f;

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
				StatId.Duration,
				duration,
				this
			);

			stats.AddBase(
				StatId.Size,
				size,
				this
			);
		}
	}
}