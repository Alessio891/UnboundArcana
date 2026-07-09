using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;

namespace UnboundArcana.Spells.Behaviors.Projectile
{
	public class ProjectileBehavior : SpellBehavior
	{
		private ProjectileBehaviorDefinition definition;

		public void InitializeDefinition(ProjectileBehaviorDefinition definition)
		{
			this.definition = definition;
		}

		public override void Cast()
		{
			ProjectileRuntimeObject projectile = new();

			spell.AddRuntimeObject(projectile);

			GameObject instance = Object.Instantiate(
				definition.projectilePrefab
			);

			ProjectileView view = instance.GetComponent<ProjectileView>();

			view.Initialize(projectile);
		}
	}
}