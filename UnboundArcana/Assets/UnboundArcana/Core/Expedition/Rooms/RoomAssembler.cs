using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class RoomAssembler
	{
		private readonly List<RoomSection> availableSections;

		public RoomAssembler(List<RoomSection> availableSections)
		{
			this.availableSections = availableSections;
		}

		public GeneratedRoomLayout Generate(int sectionCount)
		{
			var layout = new GeneratedRoomLayout();

			if (availableSections.Count == 0)
				return layout;

			var first = availableSections[
				Random.Range(0, availableSections.Count)
			];

			var firstSection = new GeneratedSection
			{
				SectionId = first.SectionId,
				Template = first,
				CellPosition = Vector2Int.zero
			};

			layout.Sections.Add(firstSection);

			int attempts = 0;

			while (layout.Sections.Count < sectionCount && attempts < 100)
			{
				attempts++;

				var baseSection = layout.Sections[
					Random.Range(0, layout.Sections.Count)
				];

				int connectorIndex = GetAvailableConnectorIndex(baseSection);

				if (connectorIndex < 0)
					continue;

				var connector =
					baseSection.Template.Connectors[connectorIndex];

				var candidate = availableSections[
					Random.Range(0, availableSections.Count)
				];

				if (candidate == baseSection.Template)
					continue;

				int candidateConnectorIndex =
					FindCompatibleConnectorIndex(candidate, connector);

				if (candidateConnectorIndex < 0)
					continue;

				var candidateConnector =
					candidate.Connectors[candidateConnectorIndex];

				Vector2Int position =
					CalculatePosition(
						baseSection,
						connector,
						candidateConnector
					);

				var generated = new GeneratedSection
				{
					SectionId = candidate.SectionId,
					Template = candidate,
					CellPosition = position
				};
				
				if (Overlaps(layout, generated))
					continue;

				baseSection.UsedConnectorIndices.Add(connectorIndex);
				generated.UsedConnectorIndices.Add(candidateConnectorIndex);

				layout.Sections.Add(generated);

				layout.Connections.Add(new RoomConnection
				{
					A = baseSection,
					ConnectorAIndex = connectorIndex,
					B = generated,
					ConnectorBIndex = candidateConnectorIndex
				});
			}

			Debug.Log($"Generated sections: {layout.Sections.Count}");

			return layout;
		}

		private int FindCompatibleConnectorIndex(
			RoomSection section,
			RoomSectionConnector target)
		{
			for (int i = 0; i < section.Connectors.Count; i++)
			{
				var connector = section.Connectors[i];

				if (connector.Type == target.Type &&
					connector.Direction == target.Direction.Opposite())
				{
					return i;
				}
			}

			return -1;
		}

		private int GetAvailableConnectorIndex(
			GeneratedSection section)
		{
			List<int> available = new();

			for (int i = 0; i < section.Template.Connectors.Count; i++)
			{
				if (!section.UsedConnectorIndices.Contains(i))
					available.Add(i);
			}

			if (available.Count == 0)
				return -1;

			return available[
				Random.Range(0, available.Count)
			];
		}

		private Vector2Int CalculatePosition(
	GeneratedSection targetSection,
	RoomSectionConnector target,
	RoomSectionConnector source)
		{
			Vector2Int connectionPoint =
				targetSection.CellPosition +
				target.CellPosition +
				target.GetDirectionOffset();

			return connectionPoint - source.CellPosition;
		}

		private bool Overlaps(
	GeneratedRoomLayout layout,
	GeneratedSection candidate)
		{
			HashSet<Vector2Int> occupied =
				new();

			foreach (var section in layout.Sections)
			{
				foreach (var cell in section.Template.Footprint.GetCells())
				{
					occupied.Add(
						section.CellPosition + cell);
				}
			}

			foreach (var cell in candidate.Template.Footprint.GetCells())
			{
				Vector2Int worldCell =
					candidate.CellPosition + cell;

				if (occupied.Contains(worldCell))
					return true;
			}

			return false;
		}
	}
}