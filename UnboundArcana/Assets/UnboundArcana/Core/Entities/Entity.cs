using System;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Entities.Statuses;
using UnityEngine;

namespace UnboundArcana.Core.Entities
{
	[RequireComponent(typeof(StatusController))]
	public class Entity : MonoBehaviour
	{
		[SerializeField]
		private EntityDefinition definition;
		public EntityEventBus Events { get; } = new();
		public EntityStats Stats { get; private set; }
		public StatusController Status { get; private set; }
		public EntityDefinition Definition => definition;

		private void Awake()
		{
			Stats = new EntityStats();
			Status = GetComponent<StatusController>();
			GetComponent<SpellCaster>()?.InitializeLoadout(definition.initialSpells);
			InitializeStats();
			Events.Subscribe<EntityDamagedEvent>(OnEntityDamaged);
		}

		private void OnEntityDamaged(EntityDamagedEvent evt)
		{
			Debug.Log("Entity Damaged!");
			GameRuntimeManager.Instance.Events.Publish(evt);
		}

		private void InitializeStats()
		{
			Stats.Set(
				EntityStatId.MaxHealth,
				definition.maxHealth
			);

			Stats.Set(
				EntityStatId.MoveSpeed,
				definition.moveSpeed
			);

			Stats.Set(
				EntityStatId.CastSpeed,
				definition.castSpeed
			);

			Stats.Set(
				EntityStatId.Armor,
				definition.armor
			);
		}
	}
}