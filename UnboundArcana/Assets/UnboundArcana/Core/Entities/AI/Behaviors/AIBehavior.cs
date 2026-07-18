using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	public abstract class AIBehavior
	{
		protected AIController Controller { get; private set; }

		public void Initialize(
			AIController controller)
		{
			Controller = controller;
			OnInitialize();
		}

		public void Tick()
		{
			OnTick();
		}

		protected virtual void OnInitialize()
		{
		}

		protected abstract void OnTick();
	}
}