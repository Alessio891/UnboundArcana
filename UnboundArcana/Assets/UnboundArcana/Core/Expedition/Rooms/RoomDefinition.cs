using System.Collections.Generic;
using UnityEngine;
using static iTween;

namespace UnboundArcana.Core.Rooms
{
	public enum RoomType
	{
		Combat,
		Elite,
		Reward,
		Event,
		Boss,
		Laboratory
	}

	[CreateAssetMenu(
		menuName = "Unbound Arcana/Rooms/Room Definition")]
	public class RoomDefinition : ScriptableObject
	{
		[SerializeField]
		private string roomId;

		[SerializeField]
		private RoomType type;

		[SerializeField]
		private int minSections = 4;

		[SerializeField]
		private int maxSections = 4;

		[SerializeField]
		private List<RoomSection> availableSections = new();
		[SerializeField]
		private RoomBehaviour behaviour;
		[SerializeField]
		private EncounterDefinition encounter;

		[SerializeField]
		private RoomObjective objective;
		public RoomObjective Objective => objective;
		public EncounterDefinition Encounter => encounter;
		public RoomBehaviour Behaviour => behaviour;
		public string RoomId => roomId;
		public RoomType Type => type;

		public int MinSections => minSections;
		public int MaxSections => maxSections;

		public IReadOnlyList<RoomSection> AvailableSections =>
			availableSections;

		public int GetSectionCount()
		{
			return Random.Range(
				minSections,
				maxSections + 1);
		}
	}
}
