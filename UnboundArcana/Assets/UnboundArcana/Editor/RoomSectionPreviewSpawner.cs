using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionPreviewSpawner
	{
		private static readonly List<RoomSectionConnector> previewConnectors = new();

		public static IReadOnlyList<RoomSectionConnector> PreviewConnectors => previewConnectors;

		private const string PreviewRootName =
			"__RoomSectionPreview";

		public static void Spawn(
			RoomSectionConnector connector,
			RoomSectionPreviewLibrary library)
		{
			Clear();
			previewConnectors.Clear();

			if (connector == null ||
				library == null)
			{
				return;
			}

			GameObject root =
				new GameObject(
					PreviewRootName);

			root.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;

			int index = 0;

			foreach (RoomSection section in library.Sections)
			{
				for (int i = 0; i < section.Connectors.Count; i++)
				{
					RoomSectionConnector candidate = section.Connectors[i];
					if (!IsCompatible(
						connector,
						candidate))
					{
						Debug.Log($"Connector {candidate.name} of section {section.SectionId} is not compatible");
						continue;
					}

					float cellSize =
	GetCellSize(connector);

					Vector2Int position =
						CalculatePosition(
							connector,
							candidate);

					RoomSection preview =
						Object.Instantiate(
							section,
							connector.transform.parent.position +
							new Vector3(
								position.x * cellSize,
								position.y * cellSize,
								0),
							Quaternion.identity,
							root.transform);

					preview.name =
						$"{section.SectionId}_Preview";
					
					RoomSectionConnector previewConnector = preview.Connectors[i];
					previewConnectors.Add(
						previewConnector);

					SetupGhost(preview);

					preview.transform.position +=
						new Vector3(
							index * 8,
							0,
							0);

					index++;

					break;
				}
			}

			
		}
		private static float GetCellSize(
			RoomSectionConnector connector)
		{
			Grid grid =
				connector.GetComponentInParent<Grid>();

			if (grid == null)
				return 1f;

			return grid.cellSize.x;
		}
		public static void Clear()
		{
			previewConnectors.Clear();

			GameObject existing =
				GameObject.Find(
					PreviewRootName);

			if (existing != null)
			{
				Object.DestroyImmediate(existing);
			}
		}
		public static void ApplyOpen()
		{
			foreach (RoomSectionConnector connector in previewConnectors)
			{
				connector.TilemapOverride?.ApplyOpen();
			}
		}

		public static void ApplyClosed()
		{
			foreach (RoomSectionConnector connector in previewConnectors)
			{
				connector.TilemapOverride?.ApplyClosed();
			}
		}
		private static bool IsCompatible(
			RoomSectionConnector a,
			RoomSectionConnector b)
		{
			return a.Shape == b.Shape &&
				a.Direction == b.Direction.Opposite();
		}

		private static Vector2Int CalculatePosition(
	RoomSectionConnector target,
	RoomSectionConnector source)
		{
			Grid grid =
				target.GetComponentInParent<Grid>();

			Vector3Int targetWorldCell =
				grid.WorldToCell(
					target.transform.position);

			Vector2Int connectionCell =
				new Vector2Int(
					targetWorldCell.x,
					targetWorldCell.y)
				+
				target.GetDirectionOffset();

			return connectionCell -
				source.CellPosition;
		}

		private static void SetupGhost(
			RoomSection section)
		{
			foreach (var renderer in
				section.GetComponentsInChildren<Renderer>())
			{
				if (renderer.sharedMaterial == null)
					continue;

				Material material =
					new Material(
						renderer.sharedMaterial);

				material.color =
					new Color(
						1f,
						1f,
						1f,
						0.7f);

				renderer.sharedMaterial =
					material;
			}
		}
	}
}