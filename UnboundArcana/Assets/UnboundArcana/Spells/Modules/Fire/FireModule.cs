using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Fire
{
	public class FireModule : SpellModule
	{
		private readonly FireModuleDefinition definition;
		private GameEventBus gameEvents;
		public FireModule(FireModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			gameEvents = spell.GameEvents;
			Events.Subscribe<HitEvent>(OnHit);
		}

		private void OnHit(HitEvent hitEvent)
		{
			gameEvents.Publish(
				new DamageEvent(
					spell.Owner,
					hitEvent.Target,
					definition.damage,
					DamageType.Fire
				)
			);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
		}
	}
}