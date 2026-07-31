using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;

namespace UnboundArcana.Spells.Modules.ExplosionOnDestroy
{
	public class ExplosionOnDestroyModule : SpellModule
	{
		private readonly ExplosionOnDestroyModuleDefinition definition;

		public ExplosionOnDestroyModule(
			ExplosionOnDestroyModuleDefinition definition)
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
			Vector3 position;

			if (eventData.RuntimeObject is AuraRuntimeObject aura)
			{
				position = aura.Position;
			} else if (eventData.RuntimeObject is ProjectileRuntimeObject projectile) {
				position = projectile.Position;
			} else {
				return;
			}

			ExplosionRuntimeObject explosion =
				new(
					position,
					definition.radius,
					definition.damage,
					definition.duration
				);

			GameObject instance = Object.Instantiate(
				definition.explosionPrefab,
				position,
				Quaternion.identity
			);

			ExplosionView view =
				instance.GetComponent<ExplosionView>();

			view.Initialize(explosion);
			spell.RegisterRuntimeObject(explosion);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<RuntimeObjectDestroyedEvent>(
				OnRuntimeObjectDestroyed
			);
		}
	}
}
