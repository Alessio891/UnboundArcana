using UnboundArcana.Core.Rooms;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AutoSpriteRendererSortingLayerDetector
{
	static AutoSpriteRendererSortingLayerDetector()
	{
		ObjectChangeEvents.changesPublished += ChangesPublished;
	}

	static void ChangesPublished(ref ObjectChangeEventStream stream)
	{
		for (int i = 0; i < stream.length; i++)
		{
			var type = stream.GetEventType(i);

			if (type == ObjectChangeKind.ChangeGameObjectParent)
			{
				ChangeGameObjectParentEventArgs evt;
				stream.GetChangeGameObjectParentEvent(i, out evt);

				var obj = EditorUtility.InstanceIDToObject(evt.instanceId) as GameObject;
				SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
				if (!renderer) return;
				RoomSection section = obj.transform.parent?.GetComponent<RoomSection>();
				if (!section) return;
				renderer.sortingLayerName = "Interactives";
				EditorUtility.SetDirty(renderer);
				Debug.Log(
					$"{obj.name} moved under {obj.transform.parent?.name}");
			}
		}
	}
}