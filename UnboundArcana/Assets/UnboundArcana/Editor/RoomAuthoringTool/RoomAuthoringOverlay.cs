using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnboundArcana.Core.Rooms.Editor
{
	[Overlay(
		typeof(SceneView),
		"Room Authoring")]
	public class RoomAuthoringOverlay : Overlay
	{
		private RoomAuthoringPanel panel;

		public override VisualElement CreatePanelContent()
		{
			panel =
				new RoomAuthoringPanel();

			return panel.Root;
		}

		public override void OnWillBeDestroyed()
		{
			panel?.Dispose();

			panel = null;
		}
	}
}