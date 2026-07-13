using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.CastSpellOnHit
{
	public class CastSpellOnHitModule : SpellModule
	{
		private readonly CastSpellOnHitModuleDefinition definition;

		public CastSpellOnHitModule(
			CastSpellOnHitModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			Events.Subscribe<HitEvent>(OnHit);
		}

		private void OnHit(HitEvent eventData)
		{
			if (definition.spellToCast == null)
			{
				return;
			}

			SpellInstance newSpell =
				SpellFactory.Create(
					new SpellConfiguration(definition.spellToCast),
					spell.Runtime,
					spell.Owner
				);

			spell.Runtime.RuntimeManager.Register(newSpell);

			newSpell.Cast(
				new CastContext(
					spell.Owner,
					eventData.Position,
					Vector3.right
				)
			);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
		}
	}
}