using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public class CastContext
	{
		public GameObject Owner { get; }
		public Vector3 Position { get; }
		public Vector3 Direction { get; }

		public CastContext(
			GameObject owner,
			Vector3 position,
			Vector3 direction)
		{
			Owner = owner;
			Position = position;
			Direction = direction.normalized;
		}
	}
}