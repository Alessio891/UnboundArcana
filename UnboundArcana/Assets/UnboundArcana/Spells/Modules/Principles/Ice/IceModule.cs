using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.Ice
{
	public class IceModule : SpellModule
	{
		private readonly IceModuleDefinition definition;
		public IceModule(
			IceModuleDefinition definition)
		{
			this.definition = definition;
		}
		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
			Events.Subscribe<HitEvent>(OnHit);
			Events.Subscribe<ProjectileSpawnedEvent>(OnProjectileSpawned);
		}

		private void OnProjectileSpawned(ProjectileSpawnedEvent e)
		{

			if (definition.projectileSprite)
			{
				e.Projectile.SetProjectileSprite(definition.projectileSprite);
				e.Projectile.SetProjectileAnimator(definition.contorller);
			}
			else
			{
				e.Projectile.SetProjectileColor(UnityEngine.Color.red);
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
			
			hitEvent.Target.Status.Apply(definition.chilledStatus, hitEvent.Owner.GetComponent<Entity>());
			
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
