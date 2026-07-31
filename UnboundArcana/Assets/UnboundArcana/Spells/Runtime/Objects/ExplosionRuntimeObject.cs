using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnityEngine;
using System.Collections.Generic;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ExplosionRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private bool exploded;
		private float lifetime;
		private float duration;
		private float radius;
		private float damage;
		public Vector3 Position => position;
		public float Duration => duration;

		public float Radius => radius * spell.Stats.Get(StatKeys.Spell.Size);

		public ExplosionRuntimeObject(
			Vector3 position,
			float radius,
			float damage,
			float duration)
		{
			this.position = position;
			this.radius = radius;
			this.damage = damage;
			this.duration = duration;
			
			lifetime = duration;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			Explode();
		}

		public override void Tick(float deltaTime)
		{
			lifetime -= deltaTime;

			if (view) {
				UpdateView(view.transform);
			}

			if (lifetime <= 0)
			{
				Destroy();
			}
		}
		public void DealDamage()
		{
			Collider2D[] hits = Physics2D.OverlapCircleAll(
				position,
				Radius
			);
			HashSet<Entity> damagedTargets = new();

			foreach (Collider2D hit in hits)
			{
				if (hit.gameObject == spell.Owner)
				{
					continue;
				}
				Entity target = hit.GetComponent<Entity>();
				if (target == null || !damagedTargets.Add(target)) continue;

				PublishHit(new HitEvent(this, target.transform.position, target, spell.Owner));
				spell.Runtime.GameEvents.Publish(
					new DamageEvent(
						spell.Owner,
						target,
						damage,
						DamageType.SpellPhysical
					)
				);
			}
		}
		private void Explode()
		{
			if (exploded)
			{
				return;
			}

			exploded = true;

			DealDamage();
		}

		public override void UpdateView(Transform transform)
		{
			SpriteRenderer renderer = transform.GetComponentInChildren<SpriteRenderer>();

			if (renderer == null || renderer.sprite == null)
			{
				transform.position = position;
				transform.localScale = Vector3.one * Radius * 2f;
				return;
			}

			Bounds bounds = renderer.sprite.bounds;
			float uniformScale = Radius * 2f / Mathf.Max(bounds.size.x, Mathf.Epsilon);
			transform.position = position - bounds.center * uniformScale;
			transform.localScale = Vector3.one * uniformScale;
		}
	}
}
