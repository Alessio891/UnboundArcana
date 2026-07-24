using UnityEditor;
using UnityEngine;

namespace UnboundArcana.Core.Rooms.Editor
{
	public static class RoomSectionPrefabUtility
	{
		private const string DefaultFolder =
			"Assets/RoomSections";


		public static GameObject GetPrefabSource(
			RoomSection section)
		{
			if (section == null)
				return null;

			return PrefabUtility.GetCorrespondingObjectFromSource(
				section.gameObject);
		}


		public static string GetPrefabPath(
			RoomSection section)
		{
			GameObject source =
				GetPrefabSource(section);

			if (source == null)
				return null;

			return AssetDatabase.GetAssetPath(
				source);
		}


		public static void CreatePrefab(
			RoomSection section)
		{
			if (section == null)
				return;


			EnsureFolder();


			string path =
				$"{DefaultFolder}/{section.SectionId}.prefab";


			path =
				AssetDatabase.GenerateUniqueAssetPath(
					path);


			GameObject prefab =
				PrefabUtility.SaveAsPrefabAsset(
					section.gameObject,
					path);


			if (prefab != null)
			{
				Debug.Log(
					$"Created RoomSection prefab: {path}");

				AssetDatabase.Refresh();
			}
		}


		public static void ApplyChanges(
			RoomSection section)
		{
			if (section == null)
				return;


			if (!PrefabUtility.IsPartOfPrefabInstance(
				section.gameObject))
			{
				Debug.LogWarning(
					"Selected section is not a prefab instance.");

				return;
			}


			PrefabUtility.ApplyPrefabInstance(
				section.gameObject,
				InteractionMode.UserAction);


			Debug.Log(
				"RoomSection prefab updated.");
		}


		public static void PingPrefab(
			RoomSection section)
		{
			GameObject source =
				GetPrefabSource(section);

			if (source == null)
				return;


			EditorGUIUtility.PingObject(
				source);
		}


		private static void EnsureFolder()
		{
			if (AssetDatabase.IsValidFolder(
				DefaultFolder))
				return;


			AssetDatabase.CreateFolder(
				"Assets",
				"RoomSections");
		}
	}
}