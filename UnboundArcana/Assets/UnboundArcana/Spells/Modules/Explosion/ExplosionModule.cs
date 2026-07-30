using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
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
			Debug.Log($"EXPLOSION CREATED AT {hitEvent.Position}");
			ExplosionRuntimeObject explosion =
				new(
					hitEvent.Position,
					definition.radius,
					definition.damage,
					definition.duration
				);

			//explosion.SetExplosionScale(definition.radius);
			GameObject instance = Object.Instantiate(
				definition.explosionPrefab,
				hitEvent.Position,
				Quaternion.identity
			);

			ExplosionView view = instance.GetComponent<ExplosionView>();

			view.Initialize(explosion);
			spell.RegisterRuntimeObject(explosion);
		}

		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
		}
		public override void ApplyStats(
			StatCollection stats)
		{
			stats.AddBase(
				StatKeys.Spell.Size,
				definition.radius,
				this
			);

			stats.AddBase(
				StatKeys.Spell.Duration,
				definition.duration,
				this
			);
		}
	}
}
