using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

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
					eventData.RuntimeObject.SetVisualAppearance(definition.projectileSprite, definition.controller, Color.white);
					break;
				case BeamRuntimeObject:
					eventData.RuntimeObject.SetVisualAppearance(definition.beamSprite, definition.beamController, new Color(1f, 0.35f, 0.1f));
					break;
				case AuraRuntimeObject:
					eventData.RuntimeObject.SetVisualAppearance(definition.auraSprite, definition.auraController, new Color(1f, 0.35f, 0.1f));
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
