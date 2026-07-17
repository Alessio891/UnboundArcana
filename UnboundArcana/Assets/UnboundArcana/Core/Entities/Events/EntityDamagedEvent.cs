using UnboundArcana.Core.Combat;

namespace UnboundArcana.Core.Entities.Events
{
	public class EntityDamagedEvent
	{
		public Entity Entity { get; }
		public DamageInfo Damage { get; }

		public EntityDamagedEvent(
			Entity entity,
			DamageInfo damage)
		{
			Entity = entity;
			Damage = damage;
		}
	}
}