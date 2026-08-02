using System;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Stats;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
	[SerializeField] Image hpBar;
	[SerializeField] Text hpText;
	Entity entity;
	private void OnEnable()
	{
		GameRuntimeManager.Instance.Events.Subscribe<PlayerSpawnedEvent>(OnPlayerSpawned);

		if (entity != null)
		{
			entity.Events.Subscribe<EntityHealthChangedEvent>(OnPlayerHealthChanged);
		}
	}
	private void OnDisable()
	{
		GameRuntimeManager.Instance.Events.Unsubscribe<PlayerSpawnedEvent>(OnPlayerSpawned);

		if (entity != null)
		{
			entity.Events.Unsubscribe<EntityHealthChangedEvent>(OnPlayerHealthChanged);
		}
	}

	private void OnPlayerSpawned(PlayerSpawnedEvent @event)
	{
		if (entity != null)
		{
			entity.Events.Unsubscribe<EntityHealthChangedEvent>(OnPlayerHealthChanged);
		}

		entity = @event.player;

		if (entity != null)
		{
			entity.Events.Subscribe<EntityHealthChangedEvent>(OnPlayerHealthChanged);
		}
	}

	private void OnPlayerHealthChanged(EntityHealthChangedEvent @event)
	{
		if (entity == null)
		{
			return;
		}

		var health = entity.GetComponent<EntityHealth>();
		float current = health.currentHealth;
		float max = entity.Stats.Get(StatKeys.Entity.MaxHealth);
		float percentage = current / max;
		hpBar.fillAmount = percentage;
		hpText.text = $"{current}/{max}";
	}
}
