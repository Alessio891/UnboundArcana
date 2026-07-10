using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ExplosionRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private float radius;
		private float damage;
		private float lifetime;

		private bool exploded;
		public Vector3 Position => position;
		public float Radius => radius;

		public ExplosionRuntimeObject(
			Vector3 position,
			float radius,
			float damage,
			float duration)
		{
			this.position = position;
			this.radius = radius;
			this.damage = damage;
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

			if (lifetime <= 0)
			{
				Destroy();
			}
		}

		private void Explode()
		{
			if (exploded)
			{
				return;
			}

			exploded = true;

			Collider2D[] hits = Physics2D.OverlapCircleAll(
				position,
				radius
			);
			Debug.Log($"Explosion hit {hits.Length} targets");
			foreach (Collider2D hit in hits)
			{
				spell.Runtime.GameEvents.Publish(
					new DamageEvent(
						spell.Owner,
						hit.gameObject,
						damage,
						DamageType.Fire
					)
				);
			}
		}
	}
}