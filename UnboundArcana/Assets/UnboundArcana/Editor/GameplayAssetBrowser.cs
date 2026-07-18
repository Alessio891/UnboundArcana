using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.AI;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.EditorTools
{
	public class GameplayAssetBrowserWindow : EditorWindow
	{
		private enum Category
		{
			All,
			Modules,
			Behaviors,
			Statuses,
			Entities,
			AI
		}

		private Category category;
		private string search = "";

		private List<ScriptableObject> assets = new();

		private Vector2 listScroll;
		private Vector2 inspectorScroll;

		private ScriptableObject selected;

		[MenuItem(
			"Tools/Unbound Arcana/Gameplay Asset Browser"
		)]
		private static void Open()
		{
			GetWindow<GameplayAssetBrowserWindow>(
				"Gameplay Assets"
			);
		}

		private void OnEnable()
		{
			Refresh();
		}

		private void OnGUI()
		{
			DrawToolbar();

			EditorGUILayout.BeginHorizontal();

			DrawAssetList();

			DrawInspector();

			EditorGUILayout.EndHorizontal();
		}

		private void DrawToolbar()
		{
			EditorGUILayout.BeginHorizontal();

			category =
				(Category)EditorGUILayout.EnumPopup(
					category,
					GUILayout.Width(120)
				);

			search =
				EditorGUILayout.TextField(
					search
				);

			if (GUILayout.Button(
				"Refresh",
				GUILayout.Width(70)
			))
			{
				Refresh();
			}

			EditorGUILayout.EndHorizontal();
		}

		private void DrawAssetList()
		{
			EditorGUILayout.BeginVertical(
				GUILayout.Width(250)
			);

			listScroll =
				EditorGUILayout.BeginScrollView(
					listScroll
				);

			foreach (ScriptableObject asset in assets)
			{
				if (!Matches(asset))
				{
					continue;
				}

				if (GUILayout.Button(
					asset.name,
					EditorStyles.label
				))
				{
					selected = asset;
				}
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}

		private void DrawInspector()
		{
			
			EditorGUILayout.BeginVertical();
			if (selected != null)
			{
				if (GUILayout.Button("Select In Project"))
				{
					Selection.activeObject = selected;
					EditorGUIUtility.PingObject(selected);
				}
			}
			inspectorScroll =
				EditorGUILayout.BeginScrollView(
					inspectorScroll
				);

			if (selected == null)
			{
				selected = null;
			}
			else
			{
				Editor editor =
					Editor.CreateEditor(
						selected
					);

				editor.OnInspectorGUI();
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}

		private bool Matches(
			ScriptableObject asset)
		{
			if (!string.IsNullOrWhiteSpace(search))
			{
				if (!asset.name
					.ToLower()
					.Contains(search.ToLower()))
				{
					return false;
				}
			}

			return category switch
			{
				Category.Modules =>
					asset is SpellModuleDefinition,

				Category.Behaviors =>
					asset is SpellBehaviorDefinition,

				Category.Statuses =>
					asset is StatusDefinition,

				Category.Entities =>
					asset is EntityDefinition,

				Category.AI =>
					asset is AIBehaviorDefinition,

				_ => true
			};
		}

		private void Refresh()
		{
			assets.Clear();
			selected = null;

			string[] guids =
				AssetDatabase.FindAssets(
					"t:ScriptableObject"
				);

			foreach (string guid in guids)
			{
				string path =
					AssetDatabase.GUIDToAssetPath(
						guid
					);

				ScriptableObject asset =
					AssetDatabase.LoadAssetAtPath<ScriptableObject>(
						path
					);

				if (asset == null)
				{
					continue;
				}

				if (IsGameplayAsset(asset))
				{
					assets.Add(asset);
				}
			}
		}

		private bool IsGameplayAsset(
			ScriptableObject asset)
		{
			return asset is SpellModuleDefinition ||
				asset is SpellBehaviorDefinition ||
				asset is StatusDefinition ||
				asset is EntityDefinition ||
				asset is AIBehaviorDefinition;
		}
	}
}