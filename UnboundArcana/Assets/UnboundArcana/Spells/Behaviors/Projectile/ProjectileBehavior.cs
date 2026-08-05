using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;
using System.Collections;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Core.Combat;

namespace UnboundArcana.Spells.Behaviors.Projectile
{
	public class ProjectileBehavior : SpellBehavior, ISpellSpawner
	{
		private ProjectileBehaviorDefinition definition;
		public float lifetime = 1.0f;

		public void InitializeDefinition(ProjectileBehaviorDefinition definition)
		{
			this.definition = definition;
			lifetime = definition.lifetime;
		}
		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			spell.Events.Subscribe<HitEvent>(OnHit);
		}
		void OnHit(HitEvent hitEvent) {
			GameRuntimeManager.Instance.Events.Publish(
					new DamageEvent(
						spell.Owner,
						hitEvent.Target,
						spell.GetChargedStat(StatKeys.Spell.Damage),
						DamageType.SpellPhysical
					)
				);
		}

		public void SpawnProjectile(SpawnContext context)
		{
			ProjectileRuntimeObject projectile = new();

			projectile.SetInitialState(
				context,
				spell.Stats.Get(StatKeys.Spell.Duration),
				definition.diameter,
				spell.Owner
			);

			float angle = Mathf.Atan2(context.Direction.y, context.Direction.x) * Mathf.Rad2Deg;
			GameObject instance = Object.Instantiate(
				definition.projectilePrefab,
				context.Position,
				Quaternion.Euler(0f, 0f, angle)
			);
			instance.SetActive(false);

			ProjectileView view = instance.GetComponent<ProjectileView>();

			view.Initialize(projectile);
			spell.RegisterRuntimeObject(projectile);
			spell.Events.Publish(
				new ProjectileSpawnedEvent(
					projectile,
					context.Position
				)
			);
			instance.SetActive(true);

		}

		public override void Cast(CastContext context)
		{
			SpawnProjectile(
				new SpawnContext(
					context.Position,
					context.Direction
				)
			);
		}

		public override void Destroy()
		{
			spell.Events.Unsubscribe<HitEvent>(OnHit);
		}

	}
}
