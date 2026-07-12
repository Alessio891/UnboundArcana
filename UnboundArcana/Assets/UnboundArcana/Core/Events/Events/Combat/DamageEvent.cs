using UnboundArcana.Core.Combat;
using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class DamageEvent : SpellEvent
	{
		public GameObject Source { get; }
		public GameObject Target { get; }
		public float Amount { get; }
		public DamageType Type { get; }

		public DamageEvent(
			GameObject source,
			GameObject target,
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