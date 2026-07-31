using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Behaviors.Aura
{
	public class AuraBehavior : SpellBehavior
	{
		private AuraBehaviorDefinition definition;

		public void InitializeDefinition(AuraBehaviorDefinition definition)
		{
			this.definition = definition;
		}

		public override void Cast(CastContext context)
		{
			AuraRuntimeObject aura = new();

			aura.SetInitialState(
				context.Position,
				spell.Stats.Get(StatKeys.Spell.Duration),
				definition.radius,
				definition.followOwner
					? spell.Owner.transform
					: null
			);

			GameObject instance = Object.Instantiate(
				definition.auraPrefab,
				context.Position,
				Quaternion.identity
			);

			AuraView view = instance.GetComponent<AuraView>();

			view.Initialize(aura);
			spell.RegisterRuntimeObject(aura);
		}
	}
}
