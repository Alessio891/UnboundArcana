using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;

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

		public override void Cast(CastContext context)
		{
			beam = new BeamRuntimeObject();

			beam.SetInitialState(
				context.Position,
				context.Direction
			);

			spell.RegisterRuntimeObject(beam);

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
	}
}
