using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionDatabase
	{
		private static List<RoomSection> sceneSections;
		private static List<RoomSection> prefabSections;

		public static RoomSection SelectedSceneSection { get; set; }

		public static RoomSection SelectedPrefabSection { get; set; }

		public static IReadOnlyList<RoomSection> SceneSections
		{
			get
			{
				Refresh();
				return sceneSections;
			}
		}

		public static IReadOnlyList<RoomSection> PrefabSections
		{
			get
			{
				Refresh();
				return prefabSections;
			}
		}

		private static void Refresh()
		{
			sceneSections =
				new List<RoomSection>();

			prefabSections =
				new List<RoomSection>();

			LoadSceneSections();
			LoadPrefabSections();
		}

		private static void LoadSceneSections()
		{
			foreach (RoomSection section in
				Object.FindObjectsByType<RoomSection>(
					FindObjectsInactive.Include,
					FindObjectsSortMode.None))
			{
				if (!sceneSections.Contains(section))
					sceneSections.Add(section);
			}
		}

		private static void LoadPrefabSections()
		{
			string[] guids =
				AssetDatabase.FindAssets(
					"t:Prefab");

			foreach (string guid in guids)
			{
				string path =
					AssetDatabase.GUIDToAssetPath(
						guid);

				GameObject prefab =
					AssetDatabase.LoadAssetAtPath<GameObject>(
						path);

				if (prefab == null)
					continue;

				RoomSection section =
					prefab.GetComponent<RoomSection>();

				if (section != null &&
					!prefabSections.Contains(section))
				{
					prefabSections.Add(section);
				}
			}
		}

		public static void Clear()
		{
			sceneSections = null;
			prefabSections = null;
		}
	}
}