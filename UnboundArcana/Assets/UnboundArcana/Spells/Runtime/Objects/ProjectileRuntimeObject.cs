using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Combat;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ProjectileRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private Vector3 direction;
		private float speed = 10f;
		private float lifetime = 2f;
		private float elapsedTime;
		private float scale = 1.0f;
		private GameObject owner;
		private SpawnContext spawnContext;

		public Vector3 Position => position;
		public Vector3 Direction => direction;
		public float Speed => speed;
		public float Scale => scale;
		public SpawnContext SpawnContext => spawnContext;

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
		}
		public void SetInitialState(
			SpawnContext context,
			float lifetime,
			GameObject owner)
		{
			this.lifetime = lifetime;
			this.spawnContext = context;
			this.position = context.Position;
			this.direction = context.Direction;
			this.owner = owner;
		}
		public override void Tick(float deltaTime)
		{
			elapsedTime += deltaTime;

			position += direction * speed * deltaTime;

			if (view != null)
			{
				view.transform.position = position;
				view.transform.localScale = new Vector3(scale, scale, scale);
			}
			if (elapsedTime >= lifetime)
			{
				Destroy();
			}
		}


		public void Hit(GameObject target)
		{
			if (target != owner && target.GetComponent<IDamageable>() != null)
			{
				spell.Events.Publish(
					new HitEvent(this, position, target, owner)
				);

				Destroy();
			}
		}
		public void SetProjectileScale(float scale) {
			this.scale = scale;
		}
		public void SetProjectileColor(Color color) {
			if (view) {
				view.GetComponentInChildren<SpriteRenderer>().color = color;
			}
		}
		public override void OnDestroyed()
		{
			base.OnDestroyed();
			spell.Events.Publish(
				new ProjectileDestroyedEvent(this)
			);
		}

	}
}