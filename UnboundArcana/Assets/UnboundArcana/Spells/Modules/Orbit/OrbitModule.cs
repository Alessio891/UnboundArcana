using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Orbit
{
	public class OrbitModule : SpellModule
	{
		private readonly OrbitModuleDefinition definition;

		public OrbitModule(
			OrbitModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(
			SpellInstance spell)
		{
			base.Initialize(spell);

			spell.Events.Subscribe<ProjectileSpawnedEvent>(
				OnProjectileSpawned
			);
		}

		private void OnProjectileSpawned(
			ProjectileSpawnedEvent eventData)
		{
			eventData.Projectile.AddModifier(
				new OrbitModifier(
					spell.Owner.transform,
					definition.radius,
					definition.angularSpeed
				)
			);
		}

		public override void Destroy()
		{
			spell.Events.Unsubscribe<ProjectileSpawnedEvent>(
				OnProjectileSpawned
			);
		}
	}
}