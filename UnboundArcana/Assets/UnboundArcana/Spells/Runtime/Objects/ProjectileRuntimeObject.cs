using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Stats;
using UnboundArcana.Core.Entities;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ProjectileRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private Vector3 direction;
		private float speed = 10f;
		private float lifetime = 2f;
		private float baseDiameter = 0.3f;
		private float elapsedTime;
		private float scale = 1.0f;
		private int remainingHits = 1;
		private bool preventDestroy;

		private GameObject owner;
		private SpawnContext spawnContext;

		public Vector3 Position => position;
		public Vector3 Direction => direction;
		public float Speed => speed;
		public float Scale => scale;
		public SpawnContext SpawnContext => spawnContext;
		public ProjectileHitHistory HitHistory => spawnContext.HitHistory;
		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
		}
		public void SetInitialState(
			SpawnContext context,
			float lifetime,
			float baseDiameter,
			GameObject owner)
		{
			this.lifetime = lifetime;
			this.baseDiameter = baseDiameter;
			this.spawnContext = context;
			this.position = context.Position;
			this.direction = context.Direction;
			this.owner = owner;
		}
		public void PreventDestroy()
		{
			preventDestroy = true;
		}
		public void SetRemainingHits(int hits)
		{
			remainingHits = Mathf.Max(1, hits);
		}
		public override void Tick(float deltaTime)
		{
			elapsedTime += deltaTime;
			
			this.speed = spell.GetChargedStat(StatKeys.Spell.Speed);
			TickModifiers(deltaTime);
			if (!modifiers.Exists(x => x.ControlsMovement))
			{
				position += direction * speed * deltaTime;
			}

			if (view != null)
			{
				SyncView();
			}
			if (elapsedTime >= lifetime)
			{
				Destroy();
			}
		}

		private void TickModifiers(float deltaTime)
		{
			base.Tick(deltaTime);
		}
		public void Hit(GameObject target)
		{
			if (target == owner)
			{
				return;
			}

			if (spawnContext.HitHistory.HasHit(target))
			{
				return;
			}

			spawnContext.HitHistory.Add(target);

			if (target.GetComponent<Entity>() != null)
			{
				HitEvent hitEvent =
					new HitEvent(
						this,
						view.transform.position,
						target.GetComponent<Entity>(),
						owner
					);
				PublishHit(hitEvent);
				remainingHits--;
				if (remainingHits <= 0 && !preventDestroy)
				{
					Destroy();
				}
			}
			else if (target.layer == LayerMask.NameToLayer("World"))
			{
				remainingHits = 0;
				Destroy();
			}
		
			

			preventDestroy = false;
		}
		public void SetProjectileDirection(Vector3 direction)
		{
			this.direction = direction.normalized;
		}
		public void SetProjectileScale(float scale) {
			this.scale = scale;
		}
		public void SetProjectileColor(Color color) {
			if (view) {
				view.ApplyVisualColor(color);
			}
		}
		public void SyncView()
		{
			if (view == null)
			{
				return;
			}

			view.transform.position = position;
			UpdateVisualScale();
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			view.transform.rotation = Quaternion.Euler(0f, 0f, angle);
		}
		public override void OnDestroyed()
		{
			base.OnDestroyed();
			spell.Events.Publish(
				new ProjectileDestroyedEvent(this)
			);
		}

		public void SetProjectilePosition(Vector3 vector3)
		{
			this.position = vector3;
		}

		private void UpdateVisualScale()
		{
			SpriteRenderer renderer = view.GetPrimaryRenderer();
			float sizeMultiplier = Mathf.Max(0.01f, scale);

			if (renderer == null || renderer.sprite == null)
			{
				view.transform.localScale = Vector3.one * sizeMultiplier;
				return;
			}

			Bounds bounds = renderer.sprite.bounds;
			float visualScale = baseDiameter * sizeMultiplier / Mathf.Max(bounds.size.x, Mathf.Epsilon);
			view.transform.localScale = Vector3.one * visualScale;
			renderer.transform.localPosition = -bounds.center;

			CircleCollider2D collider = view.GetComponent<CircleCollider2D>();
			if (collider != null)
			{
				collider.radius = bounds.size.x / 3f;
				collider.offset = Vector2.zero;
			}
		}
		public void ModifySpeed(float value)
		{
			speed = value;
		}

		public void AddSpeed(float value)
		{
			speed = Mathf.Max(0f, speed + value);
		}
	}
}
