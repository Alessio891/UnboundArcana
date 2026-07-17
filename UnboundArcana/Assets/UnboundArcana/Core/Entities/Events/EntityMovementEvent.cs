using UnityEngine;

namespace UnboundArcana.Core.Entities.Events
{
	public class EntityMovementEvent
	{
		public Entity Entity { get; }
		public Vector2 Velocity { get; }

		public EntityMovementEvent(
			Entity entity,
			Vector2 velocity)
		{
			Entity = entity;
			Velocity = velocity;
		}
	}
}