using UnityEngine;

namespace UnboundArcana.Core.Events
{
	public class EnemyKilledEvent : SpellEvent
	{
		public GameObject Enemy { get; }

		public EnemyKilledEvent(
			GameObject enemy)
		{
			Enemy = enemy;
		}
	}
}