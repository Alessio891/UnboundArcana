using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.ProjectileSpeed
{
	public class ProjectileSpeedModule : SpellModule
	{
		private readonly ProjectileSpeedModuleDefinition definition;

		public ProjectileSpeedModule(
			ProjectileSpeedModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(
			SpellInstance spell)
		{
			base.Initialize(spell);

			Events.Subscribe<ProjectileSpawnedEvent>(
				OnProjectileSpawned
			);
		}

		private void OnProjectileSpawned(
			ProjectileSpawnedEvent eventData)
		{
			eventData.Projectile.AddModifier(
				new ProjectileSpeedModifier(
					definition.acceleration
				)
			);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<ProjectileSpawnedEvent>(
				OnProjectileSpawned
			);
		}
	}
}