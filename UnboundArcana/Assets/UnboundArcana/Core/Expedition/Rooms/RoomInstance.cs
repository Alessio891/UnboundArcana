using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class RoomInstance : MonoBehaviour
	{
		private readonly List<RoomSection> sections = new();
		private readonly List<RoomMarker> markers = new();

		public IReadOnlyList<RoomSection> Sections => sections;
		public IReadOnlyList<RoomMarker> Markers => markers;

		public RoomDefinition Definition { get; private set; }
		public GeneratedRoomLayout Layout { get; private set; }

		private RoomBehaviour behaviour;
		private RoomObjective objective;
		public void Initialize(
			RoomDefinition definition,
			GeneratedRoomLayout layout)
		{
			Definition = definition;
			Layout = layout;
			objective = definition.Objective;
			behaviour = definition.Behaviour;

			sections.Clear();
			markers.Clear();

			foreach (var section in layout.Sections)
			{
				var instance = section.Instance;

				if (instance == null)
					continue;

				sections.Add(instance);

				var sectionMarkers =
					instance.GetComponentsInChildren<RoomMarker>();

				markers.AddRange(sectionMarkers);
			}
		}
		public void StartObjective()
		{
			objective?.StartObjective(this);
		}

		public void TickObjective()
		{
			objective?.Tick(this);
		}

		public void StopObjective()
		{
			objective?.StopObjective(this);
		}
		public void StartRoom()
		{
			behaviour?.StartRoom(this);

			GameRuntimeManager.Instance.Events.Publish(
				new RoomStartedEvent(this));
		}

		public void Complete()
		{
			Debug.Log(
				$"Room completed: {Definition.RoomId}");

			behaviour?.StopRoom(this);

			StopObjective();

			GameRuntimeManager.Instance.Events.Publish(
				new RoomCompletedEvent(this));
		}

		public void Tick()
		{
			behaviour?.Tick(this);
		}

		public IEnumerable<RoomMarker> GetMarkers(
			RoomMarkerType type)
		{
			foreach (var marker in markers)
			{
				if (marker.Type == type)
					yield return marker;
			}
		}
	}
}