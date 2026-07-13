using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Modifiers;

namespace UnboundArcana.Spells.Modules.Homing
{
	public class HomingModule : SpellModule
	{

		private readonly HomingModuleDefinition definition;

		public HomingModule(
			HomingModuleDefinition definition)
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
				new HomingModifier(definition.strength)
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