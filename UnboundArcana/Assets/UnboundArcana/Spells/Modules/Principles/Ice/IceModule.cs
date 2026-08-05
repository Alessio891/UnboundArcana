using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Visuals;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Modules.Ice
{
	public class IceModule : SpellModule
	{
		private readonly IceModuleDefinition definition;
		public IceModule(
			IceModuleDefinition definition)
		{
			this.definition = definition;
		}
		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			Events.Subscribe<HitEvent>(OnHit);
			Events.Subscribe<RuntimeObjectSpawnedEvent>(OnRuntimeObjectSpawned);
		}

		private void OnRuntimeObjectSpawned(RuntimeObjectSpawnedEvent eventData)
		{
			switch (eventData.RuntimeObject)
			{
				case ProjectileRuntimeObject:
					eventData.RuntimeObject.SetVisualStyle(ProceduralPalette.Ice, ProceduralPalette.IceAccent);
					break;
				case BeamRuntimeObject:
					eventData.RuntimeObject.SetVisualStyle(ProceduralPalette.Ice, ProceduralPalette.IceAccent);
					break;
				case AuraRuntimeObject:
					eventData.RuntimeObject.SetVisualStyle(ProceduralPalette.Ice, ProceduralPalette.IceAccent);
					break;
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
			if (definition.chilledStatus == null) { return; }

			Entity source = hitEvent.Owner.GetComponent<Entity>();
			hitEvent.Target.Status.Apply(definition.chilledStatus, source);
		}
		public override void ApplyStats(
			StatCollection stats)
		{
			stats.AddBase(
				StatKeys.Spell.Damage,
				definition.damage,
				this
			);
		}
		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
			Events.Unsubscribe<RuntimeObjectSpawnedEvent>(OnRuntimeObjectSpawned);
		}
	}
}
