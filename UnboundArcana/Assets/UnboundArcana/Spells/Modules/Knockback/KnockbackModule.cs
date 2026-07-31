using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Knockback
{
	public class KnockbackModule : SpellModule
	{
		private readonly KnockbackModuleDefinition definition;

		public KnockbackModule(KnockbackModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			Events.Subscribe<HitEvent>(OnHit);
		}

		private void OnHit(HitEvent hitEvent)
		{
			CharacterMotor motor = hitEvent.Target.GetComponent<CharacterMotor>();

			if (motor == null) { return; }

			Vector3 sourcePosition = hitEvent.Source switch
			{
				ProjectileRuntimeObject projectile => projectile.Position,
				ExplosionRuntimeObject explosion => explosion.Position,
				AuraRuntimeObject aura => aura.Position,
				BeamRuntimeObject beam => beam.Position,
				_ => spell.Owner.transform.position
			};

			Vector2 direction = ((Vector2)hitEvent.Target.transform.position - (Vector2)sourcePosition).normalized;
			motor.ApplyImpulse(direction * definition.force);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
		}
	}
}
