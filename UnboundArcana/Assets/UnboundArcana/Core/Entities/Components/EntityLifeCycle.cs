using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Entities;
using UnityEngine;

public class EntityLifecycle : MonoBehaviour
{
	public bool IsAlive { get; private set; } = true;

	private Entity entity;

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

		// temporary
		gameObject.SetActive(false);
	}
}