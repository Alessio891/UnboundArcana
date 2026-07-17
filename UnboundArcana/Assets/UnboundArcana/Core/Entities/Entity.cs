using UnityEngine;

namespace UnboundArcana.Core.Entities
{
	public class Entity : MonoBehaviour
	{
		[SerializeField]
		private EntityDefinition definition;
		public EntityEventBus Events { get; } = new();
		public EntityStats Stats { get; private set; }
		public EntityDefinition Definition => definition;
		private void Awake()
		{
			Stats = new EntityStats();

			InitializeStats();
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