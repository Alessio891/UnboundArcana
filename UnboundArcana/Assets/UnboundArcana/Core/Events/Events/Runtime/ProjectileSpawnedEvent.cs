using UnityEngine;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Core.Events
{
	public class ProjectileSpawnedEvent : SpellEvent
	{
		public ProjectileRuntimeObject Projectile { get; }

		public Vector3 Position { get; }

		public ProjectileSpawnedEvent(
			ProjectileRuntimeObject projectile,
			Vector3 position)
		{
			Projectile = projectile;
			Position = position;
		}
	}
}