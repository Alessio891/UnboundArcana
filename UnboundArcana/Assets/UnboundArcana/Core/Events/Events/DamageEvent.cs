using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class DamageEvent : SpellEvent
	{
		public float Amount { get; }
		public GameObject Target { get; }

		public DamageEvent(float amount, GameObject target)
		{
			Amount = amount;
			Target = target;
		}
	}
}