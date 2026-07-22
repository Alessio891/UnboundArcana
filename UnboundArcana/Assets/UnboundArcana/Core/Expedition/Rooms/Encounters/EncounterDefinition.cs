using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Rooms/Encounter Definition")]
	public class EncounterDefinition : ScriptableObject
	{
		[SerializeField]
		private List<EncounterSpawnGroup> groups = new();

		public IReadOnlyList<EncounterSpawnGroup> Groups => groups;
	}

	[System.Serializable]
	public class EncounterSpawnGroup
	{
		[SerializeField]
		private EntityDefinition entity;

		[SerializeField]
		private int count = 1;

		public EntityDefinition Entity => entity;
		public int Count => count;
	}
}