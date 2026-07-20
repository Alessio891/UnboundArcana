#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	[CustomEditor(typeof(RoomSectionConnector))]
	public class RoomSectionConnectorEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var connector = (RoomSectionConnector)target;

			if (GUILayout.Button("Snap Cell Position"))
			{
				var grid = connector.GetComponentInParent<Grid>();

				if (grid == null)
				{
					Debug.LogWarning("No Grid found in parent hierarchy.");
					return;
				}

				Undo.RecordObject(
					connector.transform,
					"Snap Connector To Grid"
				);

				Undo.RecordObject(
					connector,
					"Update Connector Cell Position"
				);

				connector.SnapToGrid(grid);

				EditorUtility.SetDirty(connector);
			}
		}
	}
}
#endif