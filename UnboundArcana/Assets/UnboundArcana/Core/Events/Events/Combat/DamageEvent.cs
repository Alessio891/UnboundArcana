using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class DamageEvent : SpellEvent
	{
		public GameObject Source { get; }
		public Entity Target { get; }
		public float Amount { get; }
		public DamageType Type { get; }

		public DamageEvent(
			GameObject source,
			Entity target,
			float amount,
			DamageType type)
		{
			Source = source;
			Target = target;
			Amount = amount;
			Type = type;
		}
	}
}