using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Ice
{
	public class IceModule : SpellModule
	{
		private readonly IceModuleDefinition definition;
		private GameEventBus gameEvents;
		public IceModule(
			IceModuleDefinition definition)
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

		private void OnProjectileSpawned(ProjectileSpawnedEvent e)
		{

			if (definition.projectileSprite)
			{
				e.Projectile.SetProjectileSprite(definition.projectileSprite);
			}
			else
			{
				e.Projectile.SetProjectileColor(UnityEngine.Color.red);
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
			if (!hitEvent.Target.Status.Has(definition.chilledStatus))
			{
				hitEvent.Target.Status.Apply(definition.chilledStatus, hitEvent.Owner.GetComponent<Entity>());
			}
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