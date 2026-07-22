using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnboundArcana.Core.Rooms
{
	public class RoomDebugView : MonoBehaviour
	{
		[SerializeField]
		private bool drawSectionLabels = true;

		[SerializeField]
		private bool drawBounds = true;

		[SerializeField]
		private bool drawMarkers = true;

		[SerializeField]
		private bool drawConnections = true;

		private RoomInstance room;

		public void Initialize(RoomInstance room)
		{
			this.room = room;
		}

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			if (room == null)
				return;

			foreach (var section in room.Sections)
			{
				DrawSection(section);
			}
		}

		private void DrawSection(RoomSection section)
		{
			//if (drawBounds)
			//{
			//	float hue = Mathf.Abs(section.SectionId.GetHashCode() % 1000) / 1000f;

			//	Gizmos.color =	Color.HSVToRGB(hue, 0.6f, 1f);

			//	foreach (var cell in section.GetFootprintCells())
			//	{
			//		Vector3 center =
			//			section.transform
			//			.GetComponentInChildren<Grid>()
			//			.GetCellCenterWorld(
			//				new Vector3Int(cell.x, cell.y, 0));

			//		Gizmos.DrawWireCube(
			//			center,
			//			section.GetComponentInChildren<Grid>().cellSize);
			//	}
			//}

			if (drawSectionLabels)
			{
				Handles.color = Color.white;

				GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
				style.fontSize = 28;
				style.alignment = TextAnchor.MiddleCenter;
				style.normal.textColor = Color.white;

				Handles.Label(
					GetSectionCenter(section),
					section.SectionId,
					style);
			}

			if (drawMarkers)
			{
				foreach (var marker in section.Markers)
				{
					Gizmos.color = GetMarkerColor(marker.Type);

					Gizmos.DrawSphere(
						marker.transform.position,
						0.08f);

					Handles.Label(
						marker.transform.position + Vector3.up * 0.1f,
						marker.Type.ToString());
				}
			}

			if (drawConnections)
			{
				foreach (var connector in section.Connectors)
				{
					if (connector.ConnectedTo == null)
						continue;

					Gizmos.color = Color.green;

					Gizmos.DrawLine(
						connector.transform.position,
						connector.ConnectedTo.transform.position);
				}
			}
			Color color = GetSectionColor(section);

			Color fill = color;
			fill.a = 0.18f;

			Color wire = color;
			wire.a = 1f;
			Grid grid = section.GetComponentInChildren<Grid>();
			foreach (var cell in section.GetFootprintCells())
			{
				Vector3 center =
					grid.GetCellCenterWorld(
						new Vector3Int(cell.x, cell.y, 0));

				Gizmos.color = fill;
				Gizmos.DrawCube(
					center,
					grid.cellSize * 0.95f);

				Gizmos.color = wire;
				Gizmos.DrawWireCube(
					center,
					grid.cellSize);
			}
		}
		private Color GetSectionColor(RoomSection section)
		{
			int hash = Mathf.Abs(section.SectionId.GetHashCode());

			float hue = (hash % 360) / 360f;

			return Color.HSVToRGB(
				hue,
				0.65f,
				1f);
		}
		private Vector3 GetSectionCenter(RoomSection section)
		{
			Grid grid = section.GetComponentInChildren<Grid>();

			Bounds bounds = new Bounds();
			bool first = true;

			foreach (var cell in section.GetFootprintCells())
			{
				Vector3 center =
					grid.GetCellCenterWorld(
						new Vector3Int(cell.x, cell.y, 0));

				if (first)
				{
					bounds = new Bounds(center, Vector3.zero);
					first = false;
				}
				else
				{
					bounds.Encapsulate(center);
				}
			}

			return bounds.center;
		}
		private Color GetMarkerColor(RoomMarkerType type)
		{
			return type switch
			{
				RoomMarkerType.PlayerStart => Color.blue,
				RoomMarkerType.EnemySpawn => Color.red,
				RoomMarkerType.Portal => Color.green,
				RoomMarkerType.Event => Color.yellow,
				RoomMarkerType.Decoration => Color.gray,
				_ => Color.white
			};
		}
#endif
	}
}