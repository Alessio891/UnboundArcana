using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ExplosionRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private bool exploded;
		private float lifetime;
		private float duration;
		public Vector3 Position => position;
		public float Duration => duration;

		public float Radius => spell.Stats.Get(StatId.Size);

		public ExplosionRuntimeObject(
			Vector3 position,
			float radius,
			float damage,
			float duration)
		{
			this.position = position;
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
				float scale = spell.Stats.Get(StatId.Size);
				view.transform.position = position;
				view.transform.localScale = new Vector3(scale, scale, scale);
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

			foreach (Collider2D hit in hits)
			{
				if (hit.gameObject == spell.Owner)
				{
					continue;
				}
				if (hit.GetComponent<Entity>() == null) continue;

				spell.Runtime.GameEvents.Publish(
					new DamageEvent(
						spell.Owner,
						hit.GetComponent<Entity>(),
						spell.Stats.Get(StatId.Damage),
						DamageType.Fire
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
	}
}