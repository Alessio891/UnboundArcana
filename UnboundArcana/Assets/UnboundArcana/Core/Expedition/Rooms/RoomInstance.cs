using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
		private bool behaviourStarted;
		private bool objectiveStarted;
		private bool isDisposed;

		private bool isCompleted;
		public bool IsCompleted => isCompleted;
		public void Initialize(
	RoomDefinition definition,
	GeneratedRoomLayout layout)
		{
			Definition = definition;
			Layout = layout;

			behaviour = definition.Behaviour;
			objective = definition.Objective;

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

			ApplyConnectorStates();
		}
		private void ApplyConnectorStates()
		{
			foreach (var section in Layout.Sections)
			{
				if (section.Instance == null)
					continue;

				var connectors =
					section.Instance.Connectors;

				for (int i = 0; i < connectors.Count; i++)
				{
					RoomSectionConnector connector =
						connectors[i];

					bool connected =
						section.UsedConnectorIndices.Contains(i);

					connector.SetConnected(
						connected);
				}
			}
		}
		public RoomSection GetSectionAtWorldPosition(Vector3 worldPosition)
		{
			foreach (RoomSection section in sections)
			{
				if (section.ContainsWorldPosition(worldPosition))
					return section;
			}

			return null;
		}
		public void StartObjective()
		{
			if (isDisposed || objectiveStarted)
				return;

			objectiveStarted = true;
			objective?.StartObjective(this);
		}

		public void TickObjective()
		{
			objective?.Tick(this);
		}

		public void StopObjective()
		{
			if (!objectiveStarted)
				return;

			objectiveStarted = false;
			objective?.StopObjective(this);
		}
		public void StopRoom()
		{
			StopAllCoroutines();

			if (behaviourStarted)
			{
				behaviourStarted = false;
				behaviour?.StopRoom(this);
			}

			StopObjective();
		}
		public void PrepareForDestruction()
		{
			if (isDisposed)
				return;

			isDisposed = true;
			StopRoom();
			gameObject.SetActive(false);
		}
		public void StartRoom()
		{
			if (isDisposed)
				return;

			StartCoroutine(StartConstructEffect());
			
		}
		IEnumerator StartConstructEffect() {
			float value = 0.0f;
			
			while(true) {
				foreach (TilemapRenderer r in GetComponentsInChildren<TilemapRenderer>())
				{
					r.material.SetFloat("_Progress", value);
				}
				foreach (RoomSection r in GetComponentsInChildren<RoomSection>())
				{
					foreach(SpriteRenderer renderer in r.Props)
						renderer.material.SetFloat("_Progress", value);
				}
				value += 0.6f * Time.deltaTime;
				yield return null;
				if (value >= 1.0f) {
					break;
				}
			}
			if (isDisposed)
				yield break;

			behaviourStarted = true;
			behaviour?.StartRoom(this);

			if (isDisposed)
				yield break;

			GameRuntimeManager.Instance.Events.Publish(
				new RoomStartedEvent(this));
		}
		public void Complete()
		{
			if (isCompleted || isDisposed)
				return;

			isCompleted = true;

			Debug.Log(
				$"Room completed: {Definition.RoomId}");

			StopRoom();

			GameRuntimeManager.Instance.Events.Publish(
				new RoomCompletedEvent(this));
			
		}

		public IEnumerator StartDeconstructEffect() {
			float value = 1.0f;

			while (true)
			{
				foreach (TilemapRenderer r in GetComponentsInChildren<TilemapRenderer>())
				{
					r.material.SetFloat("_Progress", value);
				}
				foreach (RoomSection r in GetComponentsInChildren<RoomSection>())
				{
					foreach (SpriteRenderer renderer in r.Props)
						renderer.material.SetFloat("_Progress", value);
				}
				value -= 0.3f * Time.deltaTime;
				yield return null;
				if (value <= 0.0f)
				{
					break;
				}
			}
		}

		public void Tick()
		{
			if (behaviourStarted && !isDisposed)
				behaviour?.Tick(this);
		}

		private void OnDestroy()
		{
			StopRoom();
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
