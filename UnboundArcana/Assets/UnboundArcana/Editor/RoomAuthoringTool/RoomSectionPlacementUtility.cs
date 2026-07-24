using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionPlacementUtility
	{
		public static void Spawn(
			RoomSection section)
		{
			if (section == null)
			{
				Debug.LogWarning(
					"No RoomSection selected.");

				return;
			}

			RoomSection instance =
				(Object.Instantiate(
					section.gameObject))
				.GetComponent<RoomSection>();

			instance.name =
				section.SectionId;

			Undo.RegisterCreatedObjectUndo(
				instance.gameObject,
				"Spawn Room Section");

			instance.transform.position =
				Vector3.zero;

			Selection.activeGameObject =
				instance.gameObject;

			EditorGUIUtility.PingObject(
				instance.gameObject);
		}
	}
}