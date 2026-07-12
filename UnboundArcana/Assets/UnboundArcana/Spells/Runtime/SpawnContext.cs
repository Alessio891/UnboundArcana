using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Runtime
{
	public class SpawnContext
	{
		public Vector3 Position { get; }
		public Vector3 Direction { get; }
		public bool PropagateModifiers { get; }
		public ProjectileHitHistory HitHistory { get; }

		public SpawnContext(
			Vector3 position,
			Vector3 direction,
			bool propagateModifiers = true,
			ProjectileHitHistory hitHistory = null)
		{
			Position = position;
			Direction = direction;
			PropagateModifiers = propagateModifiers;
			HitHistory = hitHistory ?? new ProjectileHitHistory();
		}
	}
}