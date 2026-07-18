using UnboundArcana.Core.Entities;

namespace UnboundArcana.Core.Entities.Statuses
{
	public abstract class StatusInstance
	{
		protected Entity target;
		protected Entity source;

		public StatusDefinition Definition { get; }
		public bool IsPermanent { get; protected set; }
		public float RemainingDuration { get; protected set; }

		public int Stacks { get; protected set; }

		protected StatusInstance(
			StatusDefinition definition)
		{
			Definition = definition;
			RemainingDuration = definition.Duration;
			Stacks = 1;
		}

		public virtual void Initialize(
			Entity target,
			Entity source)
		{
			this.target = target;
			this.source = source;
		}

		public virtual void Tick(
			float deltaTime)
		{
			if (IsPermanent)
			{
				return;
			}

			RemainingDuration -= deltaTime;
		}

		public virtual void Refresh()
		{
			RemainingDuration = Definition.Duration;
		}

		public virtual void AddStack()
		{
			if (Stacks < Definition.MaxStacks)
			{
				Stacks++;
			}

			Refresh();
		}

		public virtual void OnRemove() { }

		public bool IsExpired =>
			RemainingDuration <= 0;
	}
}