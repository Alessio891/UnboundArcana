using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public class SpawnContext
	{
		public Vector3 Position { get; }
		public Vector3 Direction { get; }
		public bool PropagateModifiers { get; }

		public SpawnContext(
			Vector3 position,
			Vector3 direction,
			bool propagateModifiers = true)
		{
			Position = position;
			Direction = direction;
			PropagateModifiers = propagateModifiers;
		}
	}
}