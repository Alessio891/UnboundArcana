using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;
using System.Collections;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnityEditor.PackageManager;
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
						spell.Stats.Get(StatId.Damage),
						DamageType.SpellPhysical
					)
				);
		}

		public void SpawnProjectile(SpawnContext context)
		{
			ProjectileRuntimeObject projectile = new();

			projectile.SetInitialState(
				context,
				lifetime,
				spell.Owner
			);

			spell.RegisterRuntimeObject(projectile);
			
			GameObject instance = Object.Instantiate(
				definition.projectilePrefab
			);

			ProjectileView view = instance.GetComponent<ProjectileView>();

			view.Initialize(projectile);
			spell.Events.Publish(
				new ProjectileSpawnedEvent(
					projectile,
					context.Position
				)
			);

			float scale = spell.Stats.Get(StatId.Size);
			if (scale <= 0) scale = 1.0f;
			view.transform.localScale = new Vector3(scale, scale, scale);
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

	}
}