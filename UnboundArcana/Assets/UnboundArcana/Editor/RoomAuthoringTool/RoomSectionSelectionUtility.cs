using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionSelectionUtility
	{
		public static void Select(
			RoomSection section)
		{
			if (section == null)
				return;

			Selection.activeGameObject =
				section.gameObject;

			EditorGUIUtility.PingObject(
				section.gameObject);
		}

		public static void Frame(
			RoomSection section)
		{
			if (section == null)
				return;

			Selection.activeGameObject =
				section.gameObject;

			SceneView.lastActiveSceneView?
				.FrameSelected();
		}

		public static void SetEnabled(
			RoomSection section,
			bool enabled)
		{
			if (section == null)
				return;

			Undo.RecordObject(
				section.gameObject,
				"Toggle Room Section");

			section.gameObject.SetActive(
				enabled);
		}
	}
}