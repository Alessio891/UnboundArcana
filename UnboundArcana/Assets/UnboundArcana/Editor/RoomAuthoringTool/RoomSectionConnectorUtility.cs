using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionConnectorUtility
	{
		public static RoomSectionConnector CreateConnector(
			RoomSection section)
		{
			if (section == null)
				return null;


			GameObject obj =
				new GameObject(
					"Connector");


			Undo.RegisterCreatedObjectUndo(
				obj,
				"Create Room Connector");


			obj.transform.SetParent(
				section.transform);


			obj.transform.localPosition =
				Vector3.zero;


			RoomSectionConnector connector =
				obj.AddComponent<RoomSectionConnector>();


			Selection.activeGameObject =
				obj;


			return connector;
		}


		public static void Select(
			RoomSectionConnector connector)
		{
			if (connector == null)
				return;


			Selection.activeGameObject =
				connector.gameObject;


			EditorGUIUtility.PingObject(
				connector.gameObject);
		}


		public static void Frame(
			RoomSectionConnector connector)
		{
			if (connector == null)
				return;


			Selection.activeGameObject =
				connector.gameObject;


			SceneView.lastActiveSceneView
				.FrameSelected();
		}


		public static void Validate(
			RoomSectionConnector connector)
		{
			if (connector == null)
				return;


			if (connector.TilemapOverride == null)
			{
				Debug.LogWarning(
					"Connector has no overlay.",
					connector);

				return;
			}


			if (!connector.TilemapOverride.IsValid())
			{
				Debug.LogWarning(
					"Connector overlay invalid.",
					connector);

				return;
			}


			Debug.Log(
				"Connector valid.",
				connector);
		}
	}
}