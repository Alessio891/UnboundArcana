using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class SpellKillEvent : SpellEvent
	{
		public GameObject Enemy { get; }

		public SpellKillEvent(
			GameObject enemy)
		{
			Enemy = enemy;
		}
	}
}