using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	[EditorTool(
		"Room Authoring Tool",
		typeof(RoomSection))]
	public class RoomAuthoringTool : EditorTool
	{
		private GUIContent icon;

		public override GUIContent toolbarIcon
		{
			get
			{
				if (icon == null)
				{
					icon = new GUIContent(
						"Room",
						"Room Authoring Tool");
				}

				return icon;
			}
		}

		public override void OnToolGUI(EditorWindow window)
		{
			if (window is not SceneView)
				return;

			//RoomAuthoringOverlay.Show();
		}
	}
}