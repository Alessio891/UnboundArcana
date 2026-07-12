using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class HitEvent : SpellEvent
	{
		public Vector3 Position { get; }
		public GameObject Target { get; }

		public GameObject Owner { get; }

		public SpellRuntimeObject Source { get; }

		public HitEvent(SpellRuntimeObject source, Vector3 position, GameObject target, GameObject owner)
		{
			Source = source;
			Position = position;
			Target = target;
			Owner = owner;
		}
	}
}