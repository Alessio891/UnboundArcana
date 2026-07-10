using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Core.Events
{
	public class ProjectileDestroyedEvent : SpellEvent
	{
		public ProjectileRuntimeObject Projectile { get; }

		public ProjectileDestroyedEvent(
			ProjectileRuntimeObject projectile)
		{
			Projectile = projectile;
		}
	}
}