using UnboundArcana.Core.Combat;
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

		public Vector3 Position => position;

		public float Radius => Stats.Get(StatId.Size);

		public ExplosionRuntimeObject(
			Vector3 position,
			float radius,
			float damage,
			float duration)
		{
			this.position = position;

			Stats.SetBase(
				StatId.Size,
				radius
			);

			Stats.SetBase(
				StatId.Damage,
				damage
			);

			Stats.SetBase(
				StatId.Duration,
				duration
			);

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
				float scale = Stats.Get(StatId.Size);
				view.transform.localScale = new Vector3(scale, scale, scale);
			}

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
				Radius
			);

			Debug.Log($"Explosion hit {hits.Length} targets");

			foreach (Collider2D hit in hits)
			{
				spell.Runtime.GameEvents.Publish(
					new DamageEvent(
						spell.Owner,
						hit.gameObject,
						Stats.Get(StatId.Damage),
						DamageType.Fire
					)
				);
			}
		}
	}
}