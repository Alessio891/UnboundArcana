using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class AuraRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private float duration;
		private float elapsedTime;

		private Transform anchor;

		public Vector3 Position => position;

		public void SetInitialState(
			Vector3 position,
			float duration,
			Transform anchor = null)
		{
			this.position = position;
			this.duration = duration;
			this.anchor = anchor;
		}

		public override void Tick(float deltaTime)
		{
			elapsedTime += deltaTime;
			base.Tick(deltaTime);
			float size = spell.Stats.Get(Core.Stats.StatId.Size);
			if (view) {
				view.transform.localScale = new Vector3(size, size, size);
			}
			if (anchor != null)
			{
				position = anchor.position;
			}
			if (view != null)
			{
				UpdateView(view.transform);
			}
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