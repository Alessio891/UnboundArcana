using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Modifiers;

namespace UnboundArcana.Spells.Modules.Piercing
{
	public class PiercingModule : SpellModule
	{
		private readonly PiercingModuleDefinition definition;

		public PiercingModule(
			PiercingModuleDefinition definition)
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
				new PiercingModifier(
					definition.additionalHits + 1
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
