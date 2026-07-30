using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Fire
{
	public class FireModule : SpellModule
	{
		private readonly FireModuleDefinition definition;
		public FireModule(FireModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			Events.Subscribe<HitEvent>(OnHit);
			Events.Subscribe<ProjectileSpawnedEvent>(OnProjectileSpawned);
		}

		private void OnProjectileSpawned(ProjectileSpawnedEvent e) {
			
			if (definition.projectileSprite)
			{
				e.Projectile.SetProjectileSprite(definition.projectileSprite);
				e.Projectile.SetProjectileAnimator(definition.controller);
			}
			else
			{
				e.Projectile.SetProjectileColor(UnityEngine.Color.red);
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
			if (!hitEvent.Target.Status.Has(definition.burningStatus))
			{
				hitEvent.Target.Status.Apply(definition.burningStatus, hitEvent.Owner.GetComponent<Entity>());
			}
		}
		public override void ApplyStats(
			StatCollection stats)
		{
			stats.AddBase(
				StatKeys.Spell.Damage,
				definition.damage,
				this
			);
		}
		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
			Events.Unsubscribe<ProjectileSpawnedEvent>(OnProjectileSpawned);
		}
	}
}
