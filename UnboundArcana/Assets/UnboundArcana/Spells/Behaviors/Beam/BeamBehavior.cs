using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Behaviors.Beam
{
	public class BeamBehavior : SpellBehavior, IContinuousSpellBehavior
	{
		private BeamBehaviorDefinition definition;
		private BeamRuntimeObject beam;

		public void InitializeDefinition(BeamBehaviorDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			spell.Events.Subscribe<HitEvent>(OnHit);
		}

		public override void Cast(CastContext context)
		{
			beam = new BeamRuntimeObject();

			beam.SetInitialState(
				context.Position,
				context.Direction,
				definition.range,
				definition.width * spell.Stats.Get(StatKeys.Spell.Size),
				definition.startupDelay,
				definition.damageInterval
			);

			GameObject instance = Object.Instantiate(
				definition.beamPrefab,
				context.Position,
				Quaternion.Euler(
					0f,
					0f,
					Mathf.Atan2(
						context.Direction.y,
						context.Direction.x
					) * Mathf.Rad2Deg
				)
			);

			BeamView view = instance.GetComponent<BeamView>();

			view.Initialize(beam);
			spell.RegisterRuntimeObject(beam);
		}

		public override void End()
		{
			if (beam != null)
			{
				beam.Destroy();
				beam = null;
			}
		}
		public override void UpdateCast(CastContext context)
		{
			if (beam != null)
			{
				beam.SetPosition(context.Position);
				beam.SetDirection(context.Direction);
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
			spell.Runtime.GameEvents.Publish(new DamageEvent(spell.Owner, hitEvent.Target, spell.Stats.Get(StatKeys.Spell.Damage), DamageType.SpellPhysical));
		}

		public override void Destroy()
		{
			spell.Events.Unsubscribe<HitEvent>(OnHit);
		}
	}
}
