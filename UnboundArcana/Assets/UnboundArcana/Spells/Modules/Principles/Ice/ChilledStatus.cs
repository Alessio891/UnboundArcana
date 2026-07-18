namespace UnboundArcana.Core.Entities.Statuses
{
	public class ChilledStatus : StatusInstance
	{
		float originalSpeed = 0.0f;
		public ChilledStatus(
			StatusDefinition definition)
			: base(definition)
		{
		}

		public override void Initialize(Entity target, Entity source)
		{
			base.Initialize(target, source);
			originalSpeed = target.Stats.Get(EntityStatId.MoveSpeed);
			target.Stats.Set(EntityStatId.MoveSpeed, 0.1f);
		}

		public override void OnRemove()
		{
			base.OnRemove();
			target.Stats.Set(EntityStatId.MoveSpeed, originalSpeed);
		}
	}
}