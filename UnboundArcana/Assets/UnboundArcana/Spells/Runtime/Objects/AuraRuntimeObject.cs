using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class AuraRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private float duration;
		private float elapsedTime;

		public Vector3 Position => position;

		public void SetInitialState(
			Vector3 position,
			float duration
		)
		{
			this.position = position;
			this.duration = duration;
		}

		public override void Tick(float deltaTime)
		{
			elapsedTime += deltaTime;

			if (elapsedTime >= duration)
			{
				Destroy();
			}
		}

		public override void UpdateView(Transform transform)
		{
			transform.position = position;
		}
	}
}