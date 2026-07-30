using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Stats;
using System;
using UnboundArcana.Core.Entities;
using UnityEngine.Tilemaps;

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
			GameObject owner)
		{
			this.lifetime = lifetime;
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
			
			this.speed = spell.Stats.Get(StatKeys.Spell.Speed);
			TickModifiers(deltaTime);
			if (!modifiers.Exists(x => x.ControlsMovement))
			{
				position += direction * speed * deltaTime;
			}

			if (view != null)
			{
				view.transform.position = position;
				view.transform.localScale = new Vector3(scale, scale, scale);
				float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				view.transform.rotation = Quaternion.Euler(0, 0, angle);
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
				NotifyHit(hitEvent);
				spell.Events.Publish(hitEvent);
				remainingHits--;
				if (remainingHits <= 0 && !preventDestroy)
				{
					Destroy();
				}
			}
			else if (target.GetComponent<TilemapCollider2D>() != null)
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
		public void SetProjectileSprite(Sprite sprite) {
			if (view) {
				view.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
			}
		}
		public void SetProjectileAnimator(RuntimeAnimatorController controller) {
			if (view) {
				view.GetComponentInChildren<Animator>().runtimeAnimatorController = controller;
			}
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

		public void SetProjectilePosition(Vector3 vector3)
		{
			this.position = vector3;
		}
		public void ModifySpeed(float value)
		{
			speed = value;
		}
	}
}
