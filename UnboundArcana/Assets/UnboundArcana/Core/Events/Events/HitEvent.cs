using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class HitEvent : SpellEvent
	{
		public Vector3 Position { get; }
		public GameObject Target { get; }

		public GameObject Owner { get; }

		public HitEvent(Vector3 position, GameObject target, GameObject owner)
		{
			Position = position;
			Target = target;
			Owner = owner;
		}
	}
}