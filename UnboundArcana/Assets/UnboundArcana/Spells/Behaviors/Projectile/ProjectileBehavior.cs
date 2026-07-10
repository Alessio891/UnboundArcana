using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;
using System.Collections;
using UnboundArcana.Core.Events;

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

		public void SpawnProjectile(SpawnContext context)
		{
			ProjectileRuntimeObject projectile = new();

			projectile.SetInitialState(
				context,
				lifetime,
				spell.Owner
			);

			spell.AddRuntimeObject(projectile);
			spell.Events.Publish(
				new ProjectileSpawnedEvent(
					projectile,
					context.Position
				)
			);
			GameObject instance = Object.Instantiate(
				definition.projectilePrefab
			);

			ProjectileView view = instance.GetComponent<ProjectileView>();

			view.Initialize(projectile);
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