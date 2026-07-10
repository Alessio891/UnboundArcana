using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;

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
				definition.duration
			);

			spell.RegisterRuntimeObject(aura);

			GameObject instance = Object.Instantiate(
				definition.auraPrefab,
				context.Position,
				Quaternion.identity
			);

			AuraView view = instance.GetComponent<AuraView>();

			view.Initialize(aura);
		}
	}
}