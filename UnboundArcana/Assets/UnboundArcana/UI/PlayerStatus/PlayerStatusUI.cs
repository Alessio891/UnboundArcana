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
	}
	private void OnDisable()
	{
		GameRuntimeManager.Instance.Events.Unsubscribe<PlayerSpawnedEvent>(OnPlayerSpawned);
	}

	private void OnPlayerSpawned(PlayerSpawnedEvent @event)
	{
		entity = @event.player;
		Debug.Log("Player Spawned, binding UI");
		entity.Events.Subscribe<EntityDamagedEvent>(OnPlayerDamaged);
	}

	private void OnPlayerDamaged(EntityDamagedEvent @event)
	{
		Debug.Log("Updating player health bar");
		var health = entity.GetComponent<EntityHealth>();
		float current = health.currentHealth;
		float max = entity.Stats.Get(StatKeys.Entity.MaxHealth);
		float percentage = current / max;
		hpBar.fillAmount = percentage;
		hpText.text = $"{current}/{max}";
	}
}
