using System;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Entities.Statuses;
using UnboundArcana.Core.Stats;
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
			InitializeStats();
			Events.Subscribe<EntityDamagedEvent>(OnEntityDamaged);
		}

		private void OnEntityDamaged(EntityDamagedEvent evt)
		{
			GameRuntimeManager.Instance.Events.Publish(evt);
		}

		private void InitializeStats()
		{
			GetComponent<SpellCaster>().InitializeLoadout(definition.initialSpells);

			Stats.AddBase(
				StatKeys.Entity.MaxHealth,
				definition.maxHealth,
				this
			);

			Stats.AddBase(
				StatKeys.Entity.MoveSpeed,
				definition.moveSpeed,
				this
			);

			Stats.AddBase(
				StatKeys.Entity.CastSpeed,
				definition.castSpeed,
				this
			);

			Stats.AddBase(
				StatKeys.Entity.Armor,
				definition.armor,
				this
			);
		}
	}
}