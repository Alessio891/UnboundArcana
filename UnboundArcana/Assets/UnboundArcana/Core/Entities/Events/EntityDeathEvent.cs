namespace UnboundArcana.Core.Entities.Events
{
	public class EntityDeathEvent
	{
		public Entity Entity { get; }

		public EntityDeathEvent(
			Entity entity)
		{
			Entity = entity;
		}
	}
}