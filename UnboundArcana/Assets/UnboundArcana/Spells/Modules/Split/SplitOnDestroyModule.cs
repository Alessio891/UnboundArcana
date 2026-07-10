using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Modules.Split
{
	public class SplitOnDestroyModule : SpellModule
	{
		private SplitOnDestroyModuleDefinition definition;

		public SplitOnDestroyModule(SplitOnDestroyModuleDefinition definition) {
			this.definition = definition;
		}

		public void InitializeDefinition(
			SplitOnDestroyModuleDefinition definition)
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
			if (eventData.RuntimeObject is not ProjectileRuntimeObject projectile)
			{
				return;
			}

			if (spell.Spawner == null)
			{
				return;
			}

			if (definition.count <= 1)
			{
				return;
			}

			if (!projectile.SpawnContext.PropagateModifiers)
			{
				return;
			}

			Vector3 direction = projectile.Direction;

			float step =
				definition.spreadAngle /
				(definition.count - 1);

			for (int i = 0; i < definition.count; i++)
			{
				float angle =
					-definition.spreadAngle / 2 +
					step * i;

				Vector3 rotatedDirection =
					Quaternion.Euler(
						0,
						0,
						angle
					) * direction;

				spell.Spawner.SpawnProjectile(
					new SpawnContext(
						projectile.Position,
						rotatedDirection,
						false
					)
				);
			}
		}

		public override void Destroy()
		{
			Events.Unsubscribe<RuntimeObjectDestroyedEvent>(
				OnRuntimeObjectDestroyed
			);
		}
	}
}