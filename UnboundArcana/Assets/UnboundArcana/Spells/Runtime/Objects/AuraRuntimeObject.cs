using UnboundArcana.Core.Stats;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class AuraRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private float duration;
		private float baseRadius;
		private float elapsedTime;
		private Vector2 visualOffset;

		private Transform anchor;

		public Vector3 Position => position;
		public float Radius => baseRadius * spell.Stats.Get(StatKeys.Spell.Size);

		public void SetAnchor(Transform anchor)
		{
			this.anchor = anchor;
		}

		public void SetVisualOffset(Vector2 offset)
		{
			visualOffset = offset;
		}

		public void SetInitialState(
			Vector3 position,
			float duration,
			float baseRadius,
			Transform anchor = null)
		{
			this.position = position;
			this.duration = duration;
			this.baseRadius = baseRadius;
			this.anchor = anchor;
		}

		public override void Tick(float deltaTime)
		{
			elapsedTime += deltaTime;
			base.Tick(deltaTime);
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
			SpriteRenderer renderer = transform.GetComponentInChildren<SpriteRenderer>();

			if (renderer == null || renderer.sprite == null)
			{
				transform.position = position + (Vector3)visualOffset;
				transform.localScale = Vector3.one * Radius * 2f;
				return;
			}

			Bounds bounds = renderer.sprite.bounds;
			float uniformScale = Radius * 2f / Mathf.Max(bounds.size.x, Mathf.Epsilon);
			Vector3 scaledCenter = bounds.center * uniformScale;
			transform.position = position + (Vector3)visualOffset - scaledCenter;
			transform.localScale = Vector3.one * uniformScale;
		}
	}
}
