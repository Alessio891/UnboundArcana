using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Fork
{
	public class ForkModule : SpellModule
	{
		private readonly ForkModuleDefinition definition;

		public ForkModule(ForkModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			Events.Subscribe<CastEvent>(OnCast);
		}

		private void OnCast(CastEvent castEvent)
		{
			if (spell.Spawner == null)
			{
				return;
			}

			Vector3 baseDirection = castEvent.Context.Direction;

			for (int i = 0; i < definition.additionalProjectiles; i++)
			{
				int pairIndex = i / 2 + 1;
				float side = i % 2 == 0 ? -1f : 1f;
				float offset = definition.angle * pairIndex * side;

				Vector3 direction = Quaternion.Euler(
					0,
					0,
					offset
				) * baseDirection;

				spell.Spawner.SpawnProjectile(
					new SpawnContext(
						castEvent.Context.Position,
						direction
					)
				);
			}
		}

		public override void Destroy()
		{
			Events.Unsubscribe<CastEvent>(OnCast);
		}
	}
}
