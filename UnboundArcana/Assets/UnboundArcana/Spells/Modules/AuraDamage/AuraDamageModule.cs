using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Modules.AuraDamage
{
	public class AuraDamageModule : SpellModule
	{
		private readonly AuraDamageModuleDefinition definition;

		public AuraDamageModule(
			AuraDamageModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(
			SpellInstance spell)
		{
			base.Initialize(spell);

			Events.Subscribe<RuntimeObjectSpawnedEvent>(
				OnRuntimeObjectSpawned
			);
		}

		private void OnRuntimeObjectSpawned(
			RuntimeObjectSpawnedEvent eventData)
		{
			if (eventData.RuntimeObject is not AuraRuntimeObject aura)
			{
				return;
			}

			aura.AddModifier(
				new AuraDamageModifier(
					spell,
					definition.damage,
					definition.interval
				)
			);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<RuntimeObjectSpawnedEvent>(
				OnRuntimeObjectSpawned
			);
		}
	}
}