using System.Collections;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Entities;
using UnityEngine;

public class EntityLifecycle : MonoBehaviour
{
	public bool IsAlive { get; private set; } = true;

	private Entity entity;
	[SerializeField] private float deathFeedbackDuration;

	private void Awake()
	{
		entity = GetComponent<Entity>();
	}

	private void OnEnable()
	{
		entity.Events.Subscribe<EntityDeathEvent>(
			OnDeath);
	}

	private void OnDisable()
	{
		entity.Events.Unsubscribe<EntityDeathEvent>(
			OnDeath);
	}

	private void OnDeath(EntityDeathEvent evt)
	{
		if (!IsAlive)
		{
			return;
		}

		IsAlive = false;
		StopGameplay();
		GameRuntimeManager.Instance.Events.Publish(evt);

		if (deathFeedbackDuration > 0f)
		{
			StartCoroutine(DestroyAfterDeathFeedback());
			return;
		}

		gameObject.SetActive(false);
	}

	private void StopGameplay()
	{
		foreach (EntityController controller in GetComponents<EntityController>())
		{
			controller.enabled = false;
		}

		if (TryGetComponent(out CharacterMotor motor))
		{
			motor.StopImmediately();
			motor.enabled = false;
		}

		if (TryGetComponent(out MeleeAttacker meleeAttacker))
		{
			meleeAttacker.CancelAttack();
			meleeAttacker.enabled = false;
		}

		if (TryGetComponent(out SpellCaster spellCaster))
		{
			spellCaster.CancelActiveSpell();
			spellCaster.enabled = false;
		}

		foreach (Collider2D entityCollider in GetComponentsInChildren<Collider2D>())
		{
			entityCollider.enabled = false;
		}
	}

	private IEnumerator DestroyAfterDeathFeedback()
	{
		yield return new WaitForSeconds(deathFeedbackDuration);
		Destroy(gameObject);
	}
}
