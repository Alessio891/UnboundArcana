using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class HitEvent : SpellEvent
	{
		public Vector3 Position { get; }
		public GameObject Target { get; }

		public HitEvent(Vector3 position, GameObject target)
		{
			Position = position;
			Target = target;
		}
	}
}