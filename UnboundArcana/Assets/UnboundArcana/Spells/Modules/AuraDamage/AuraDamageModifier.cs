using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.AuraDamage
{
	public class AuraDamageModifier : IRuntimeObjectModifier
	{
		private readonly SpellInstance spell;
		private readonly float damage;
		private readonly float interval;

		private AuraRuntimeObject aura;
		private float timer;

		public bool ControlsMovement => false;

		public AuraDamageModifier(
			SpellInstance spell,
			float damage,
			float startupDelay,
			float interval)
		{
			this.spell = spell;
			this.damage = damage;
			this.interval = interval;
			this.timer = interval - startupDelay;
		}

		public void Initialize(
			SpellRuntimeObject runtimeObject)
		{
			aura =
				runtimeObject as AuraRuntimeObject;
		}

		public void Update(float deltaTime)
		{
			if (aura == null)
			{
				return;
			}

			timer += deltaTime;

			if (timer < interval)
			{
				return;
			}

			timer -= interval;

			Collider2D[] hits = Physics2D.OverlapCircleAll(
				aura.Position,
				aura.Radius
			);

			foreach (Collider2D hit in hits)
			{
				if (hit.gameObject == spell.Owner)
				{
					continue;
				}
				if (hit.GetComponent<Entity>() == null) {
					continue;
				}

				Entity target = hit.GetComponent<Entity>();
				HitEvent hitEvent = new HitEvent(aura, target.transform.position, target, spell.Owner);
				aura.PublishHit(hitEvent);
				spell.Runtime.GameEvents.Publish(new DamageEvent(spell.Owner, target, damage, DamageType.SpellPhysical));
			}
		}

		public void OnHit(HitEvent hitEvent)
		{
		}

		public void Destroy()
		{
		}
	}
}
