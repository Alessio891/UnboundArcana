using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Fire
{
	public class FireModule : SpellModule
	{
		private readonly FireModuleDefinition definition;
		private GameEventBus gameEvents;
		public FireModule(FireModuleDefinition definition)
		{
			this.definition = definition;
		}

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			gameEvents = spell.Runtime.GameEvents;
			Events.Subscribe<HitEvent>(OnHit);
			Events.Subscribe<ProjectileSpawnedEvent>(OnProjectileSpawned);
		}

		private void OnProjectileSpawned(ProjectileSpawnedEvent e) {
			e.Projectile.SetProjectileColor(UnityEngine.Color.red);
		}

		private void OnHit(HitEvent hitEvent)
		{
			gameEvents.Publish(
				new DamageEvent(
					spell.Owner,
					hitEvent.Target,
					spell.Stats.Get(StatId.Damage),
					DamageType.Fire
				)
			);
		}
		public override void ApplyStats(
			StatCollection stats)
		{
			stats.AddBase(
				StatId.Damage,
				definition.damage,
				this
			);
		}
		public override void Destroy()
		{
			Events.Unsubscribe<HitEvent>(OnHit);
		}
	}
}