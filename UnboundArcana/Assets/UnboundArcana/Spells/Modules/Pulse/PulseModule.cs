using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Pulse
{
	public class PulseModule : SpellModule
	{
		private readonly PulseModuleDefinition definition;

		public PulseModule(
			PulseModuleDefinition definition)
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
			if (eventData.RuntimeObject is not ExplosionRuntimeObject explosion)
			{
				return;
			}
			Debug.Log("Explosion spawned, adding modifier");
			explosion.AddModifier(
				new PulseModifier(
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