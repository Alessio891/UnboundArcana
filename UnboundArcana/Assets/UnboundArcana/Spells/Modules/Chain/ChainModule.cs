using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Modifiers;

namespace UnboundArcana.Spells.Modules.Chain
{
	public class ChainModule : SpellModule
	{
		private readonly ChainModuleDefinition definition;

		public ChainModule(
			ChainModuleDefinition definition)
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
				new ChainModifier(
					definition.range, definition.maxChains
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