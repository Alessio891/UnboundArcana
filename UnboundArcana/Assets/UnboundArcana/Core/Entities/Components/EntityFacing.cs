using UnityEngine;
using UnboundArcana.Core.Entities.Events;

namespace UnboundArcana.Core.Entities.Events
{
	public class EntityFacingChangedEvent
	{
		public Entity Entity { get; }

		public Vector2 Direction { get; }

		public EntityFacingChangedEvent(
			Entity entity,
			Vector2 direction)
		{
			Entity = entity;
			Direction = direction;
		}
	}
}

namespace UnboundArcana.Core.Entities
{
	[RequireComponent(typeof(Entity))]
	public class EntityFacing : MonoBehaviour
	{
		private Entity entity;

		public Vector2 Direction { get; private set; } =
			Vector2.right;

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		public void SetDirection(Vector2 direction)
		{
			if (direction.sqrMagnitude < 0.0001f)
			{
				return;
			}

			direction.Normalize();

			if (Direction == direction)
			{
				return;
			}

			Direction = direction;

			entity.Events.Publish(
				new EntityFacingChangedEvent(
					entity,
					Direction));
		}
	}
}