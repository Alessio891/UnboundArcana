using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnityEngine;

public class EntityMeleeAttackEvent {

}

public class MeleeAttacker : MonoBehaviour
{
	Entity entity;
	TargetingComponent targeting;
	[SerializeField] float damage = 5.0f;
	private void Awake()
	{
		entity = GetComponent<Entity>();
		targeting = GetComponent<TargetingComponent>();
	}

	public void PerformMeleeAttack() {
		if (targeting.CurrentTarget != null) {
			entity.Events.Publish(new EntityMeleeAttackEvent());
			DamageInfo info = new DamageInfo(entity.gameObject, damage, DamageType.Physical);
			GameRuntimeManager.Instance.Events.Publish(new EntityDamagedEvent(targeting.CurrentTarget, info));
		}
	}
}
