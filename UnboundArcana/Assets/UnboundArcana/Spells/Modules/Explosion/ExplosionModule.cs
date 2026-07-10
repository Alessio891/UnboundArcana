using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime.Views;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Explosion
{
	public class ExplosionModule : SpellModule
	{
		private readonly ExplosionModuleDefinition definition;

		public ExplosionModule(ExplosionModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			Events.Subscribe<HitEvent>(OnHit);
		}

		private void OnHit(HitEvent hitEvent)
		{
			ExplosionRuntimeObject explosion =
				new(
					hitEvent.Position,
					definition.radius,
					definition.damage,
					definition.duration
				);

			spell.AddRuntimeObject(explosion);

			GameObject instance = Object.Instantiate(
				definition.explosionPrefab,
				hitEvent.Position,
				Quaternion.identity
			);

			ExplosionView view = instance.GetComponent<ExplosionView>();

			view.Initialize(explosion);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
		}
	}
}