using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Modules.SizeModifier;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Modules.Modifiers
{
	public class SizeModifierModule : SpellModule
	{
		private readonly SizeModifierModuleDefinition definition;

		public SizeModifierModule(
			SizeModifierModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			Events.Subscribe<RuntimeObjectSpawnedEvent>(
				OnRuntimeObjectSpawned
			);
		}

		private void OnRuntimeObjectSpawned(
			RuntimeObjectSpawnedEvent eventData)
		{
			eventData.RuntimeObject.Stats.AddModifier(
				new StatModifier(
					StatId.Size,
					definition.percentage,
					ModifierOperation.Percent,
					this
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