using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Stats;
using UnityEngine;

public class EntityMeleeAttackEvent {

}

public class MeleeAttacker : MonoBehaviour
{
	Entity entity;
	TargetingComponent targeting;
	[SerializeField] float damage = 5.0f;
	[SerializeField] float baseCooldown = 1.5f;
	
	bool onCooldown = false;
	float timer = 0.0f;
	
	Entity lastTarget;

	private bool isAttacking = false;
	public bool IsAttacking => isAttacking;

	private void Awake()
	{
		entity = GetComponent<Entity>();
		targeting = GetComponent<TargetingComponent>();
	}

	public void PerformMeleeAttack() {

		if (onCooldown) { return; }
		isAttacking = true;
		float speed = entity.Stats.Get(StatKeys.Entity.CastSpeed);
		float actualCooldown = baseCooldown / (speed > 0 ? speed : 1);
		timer = actualCooldown;
		onCooldown = true;
		if (targeting.CurrentTarget != null) {
			entity.Events.Publish(new EntityMeleeAttackEvent());
			lastTarget = targeting.CurrentTarget;
		}
	}

	private void Update()
	{
		if (onCooldown) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				onCooldown = false;
			}
		}
	}

	void OnMeleeAttackHit() {
		float dist = Vector3.Distance(transform.position, lastTarget.transform.position);
		if (dist <= 0.5f)
		{
			var health = targeting.CurrentTarget.GetComponent<EntityHealth>();
			DamageInfo info = new DamageInfo(entity.gameObject, damage, DamageType.Physical);
			if (health != null)
			{
				health.TakeDamage(info);
			}
		}
		isAttacking = false;
	}
}
