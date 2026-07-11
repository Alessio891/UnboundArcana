using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Data;

namespace UnboundArcana.Spells.Modules.CastSpellOnDestroy
{
	public class CastSpellOnDestroyModule : SpellModule
	{
		private readonly CastSpellOnDestroyModuleDefinition definition;

		public CastSpellOnDestroyModule(
			CastSpellOnDestroyModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			Events.Subscribe<RuntimeObjectDestroyedEvent>(
				OnRuntimeObjectDestroyed
			);
		}

		private void OnRuntimeObjectDestroyed(
			RuntimeObjectDestroyedEvent eventData)
		{
			if (eventData.RuntimeObject is not AuraRuntimeObject aura)
			{
				return;
			}

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
					aura.Position,
					Vector3.right
				)
			);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<RuntimeObjectDestroyedEvent>(
				OnRuntimeObjectDestroyed
			);
		}
	}
}