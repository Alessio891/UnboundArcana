namespace UnboundArcana.Core.Entities.Events
{
	public class EntityHealthChangedEvent
	{
		public Entity Entity { get; }

		public EntityHealthChangedEvent(Entity entity)
		{
			Entity = entity;
		}
	}
}
