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
	bool attackHitPending;

	private bool isAttacking = false;
	public bool IsAttacking => isAttacking;

	private void Awake()
	{
		entity = GetComponent<Entity>();
		targeting = GetComponent<TargetingComponent>();
	}

	public void PerformMeleeAttack()
	{
		if (!TryBeginAttack(out Entity target)) { return; }

		lastTarget = target;
		attackHitPending = true;
		entity.Events.Publish(new EntityMeleeAttackEvent());
	}

	public bool PerformImmediateMeleeAttack(float range)
	{
		if (!TryBeginAttack(out Entity target)) { return false; }

		lastTarget = target;
		attackHitPending = false;
		entity.Events.Publish(new EntityMeleeAttackEvent());
		ApplyDamage(target, range);
		isAttacking = false;
		return true;
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

	void OnMeleeAttackHit()
	{
		if (!attackHitPending || lastTarget == null)
		{
			isAttacking = false;
			return;
		}

		attackHitPending = false;
		ApplyDamage(lastTarget, 0.5f);
		isAttacking = false;
	}

	private bool TryBeginAttack(out Entity target)
	{
		target = targeting.CurrentTarget;

		if (onCooldown || target == null) { return false; }

		isAttacking = true;
		float speed = entity.Stats.Get(StatKeys.Entity.CastSpeed);
		float actualCooldown = baseCooldown / (speed > 0f ? speed : 1f);
		timer = actualCooldown;
		onCooldown = true;
		return true;
	}

	private void ApplyDamage(Entity target, float range)
	{
		if (target == null || Vector3.Distance(transform.position, target.transform.position) > range) { return; }

		EntityHealth health = target.GetComponent<EntityHealth>();

		if (health != null)
		{
			health.TakeDamage(new DamageInfo(entity.gameObject, damage, DamageType.Physical));
		}
	}
}
