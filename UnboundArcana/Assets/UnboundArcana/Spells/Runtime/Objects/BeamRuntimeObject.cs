using UnityEngine;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class BeamRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private Vector3 direction;
		private float range;
		private float width;
		private float damageInterval;
		private float damageTimer;
		private float currentRange;
		private readonly HashSet<Entity> hitTargets = new();

		public Vector3 Position => position;
		public Vector3 Direction => direction;
		public float CurrentRange => currentRange;
		public float Width => width;

		public void SyncView()
		{
			if (view != null)
			{
				UpdateView(view.transform);
			}
		}

		public void SetInitialState(
			Vector3 position,
			Vector3 direction,
			float range,
			float width,
			float startupDelay,
			float damageInterval
		)
		{
			this.position = position;
			this.direction = direction.normalized;
			this.range = range;
			this.currentRange = range;
			this.width = width;
			this.damageTimer = Mathf.Max(0f, startupDelay);
			this.damageInterval = Mathf.Max(0.05f, damageInterval);
		}

		public override void Tick(float deltaTime)
		{
			base.Tick(deltaTime);
			damageTimer -= deltaTime;

			if (damageTimer > 0f) { return; }

			damageTimer = damageInterval;
			hitTargets.Clear();
			currentRange = CalculateCurrentRange();
			SyncView();
			Vector2 center = position + direction * currentRange * 0.5f;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			Collider2D[] hits = Physics2D.OverlapBoxAll(center, new Vector2(currentRange, width), angle);

			foreach (Collider2D hit in hits)
			{
				Entity target = hit.GetComponent<Entity>();

				if (target == null || target.gameObject == spell.Owner || !hitTargets.Add(target)) { continue; }

				PublishHit(new HitEvent(this, target.transform.position, target, spell.Owner));
			}
		}
		public void SetPosition(Vector3 position) {
			this.position = position;
			SyncView();
		}
		public void SetDirection(Vector3 direction)
		{
			this.direction = direction.normalized;

			SyncView();
		}
		public override void UpdateView(Transform transform)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
			SpriteRenderer renderer = view.GetPrimaryRenderer();

			if (renderer == null || renderer.sprite == null)
			{
				transform.position = position + direction * currentRange * 0.5f;
				transform.rotation = rotation;
				transform.localScale = new Vector3(currentRange, width, 1f);
				return;
			}

			Bounds bounds = renderer.sprite.bounds;
			float scaleX = currentRange / Mathf.Max(bounds.size.x, Mathf.Epsilon);
			float scaleY = width / Mathf.Max(bounds.size.y, Mathf.Epsilon);
			Vector3 localAnchor = new Vector3(bounds.min.x * scaleX, bounds.center.y * scaleY, 0f);
			transform.position = position - rotation * localAnchor;
			transform.rotation = rotation;
			transform.localScale = new Vector3(scaleX, scaleY, 1f);
		}

		private float CalculateCurrentRange()
		{
			RaycastHit2D[] hits = Physics2D.RaycastAll(position, direction, range);
			float closestDistance = range;

			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider is TilemapCollider2D && hit.distance < closestDistance)
				{
					closestDistance = hit.distance;
				}
			}

			return closestDistance;
		}
	}
}
