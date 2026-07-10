using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;

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
				float offset = definition.angle * (i + 1);

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