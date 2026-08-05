using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Core.Visuals;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Modules.Fire
{
	public class FireModule : SpellModule
	{
		private readonly FireModuleDefinition definition;
		public FireModule(FireModuleDefinition definition)
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
					eventData.RuntimeObject.SetVisualStyle(ProceduralPalette.Fire, ProceduralPalette.FireAccent);
					break;
				case BeamRuntimeObject:
					eventData.RuntimeObject.SetVisualStyle(ProceduralPalette.Fire, ProceduralPalette.FireAccent);
					break;
				case AuraRuntimeObject:
					eventData.RuntimeObject.SetVisualStyle(ProceduralPalette.Fire, ProceduralPalette.FireAccent);
					break;
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
			if (definition.burningStatus != null && !hitEvent.Target.Status.Has(definition.burningStatus))
			{
				Entity source = hitEvent.Owner.GetComponent<Entity>();
				hitEvent.Target.Status.Apply(definition.burningStatus, source);
			}
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
