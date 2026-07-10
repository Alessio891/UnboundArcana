using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;
using System.Collections;

namespace UnboundArcana.Spells.Behaviors.Projectile
{
	public class ProjectileBehavior : SpellBehavior, ISpellSpawner
	{
		private ProjectileBehaviorDefinition definition;

		public void InitializeDefinition(ProjectileBehaviorDefinition definition)
		{
			this.definition = definition;
		}
		public void SpawnProjectile(
			Vector3 position,
			Vector3 direction)
		{
			ProjectileRuntimeObject projectile = new();

			projectile.SetInitialState(
				position,
				direction,
				spell.Owner
			);

			spell.AddRuntimeObject(projectile);

			GameObject instance = Object.Instantiate(
				definition.projectilePrefab
			);

			ProjectileView view = instance.GetComponent<ProjectileView>();

			view.Initialize(projectile);
		}

		public override void Cast(CastContext context)
		{
			SpawnProjectile(
				context.Position,
				context.Direction
			);
		}
	}
}