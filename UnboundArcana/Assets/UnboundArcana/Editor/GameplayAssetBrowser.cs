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

		private readonly List<ScriptableObject> assets = new();

		private Vector2 listScroll;
		private Vector2 inspectorScroll;

		private ScriptableObject selected;
		private Editor selectedEditor;

		private GUIStyle headerStyle;
		private GUIStyle titleStyle;
		private GUIStyle subtitleStyle;
		private GUIStyle cardTitleStyle;
		private GUIStyle cardSubtitleStyle;

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

		private void OnDisable()
		{
			if (selectedEditor != null)
			{
				DestroyImmediate(selectedEditor);
			}
		}

		private void OnGUI()
		{
			InitializeStyles();

			DrawToolbar();

			EditorGUILayout.BeginHorizontal();

			DrawAssetList();

			DrawInspector();

			EditorGUILayout.EndHorizontal();
		}

		private void InitializeStyles()
		{
			if (headerStyle != null)
			{
				return;
			}

			headerStyle = new GUIStyle(EditorStyles.boldLabel)
			{
				fontSize = 18
			};

			titleStyle = new GUIStyle(EditorStyles.boldLabel)
			{
				fontSize = 15
			};

			subtitleStyle = new GUIStyle(EditorStyles.miniLabel);

			cardTitleStyle = new GUIStyle(EditorStyles.boldLabel);

			cardSubtitleStyle = new GUIStyle(EditorStyles.miniLabel);
		}

		private void DrawToolbar()
		{
			EditorGUILayout.Space(4);

			EditorGUILayout.LabelField(
				"Gameplay Assets",
				headerStyle
			);

			EditorGUILayout.BeginHorizontal(
				EditorStyles.toolbar
			);

			category =
				(Category)EditorGUILayout.EnumPopup(
					category,
					EditorStyles.toolbarPopup,
					GUILayout.Width(120)
				);

			search =
				EditorGUILayout.TextField(
					search,
					EditorStyles.toolbarSearchField,
					GUILayout.ExpandWidth(true)
				);

			if (GUILayout.Button(
				"Refresh",
				EditorStyles.toolbarButton,
				GUILayout.Width(70)
			))
			{
				Refresh();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space(4);
		}

		private void DrawAssetList()
		{
			EditorGUILayout.BeginVertical(
				GUILayout.Width(300)
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

				DrawAssetCard(asset);
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}

		private void DrawAssetCard(
			ScriptableObject asset)
		{
			bool isSelected =
				selected == asset;

			GUIStyle style =
				isSelected ?
				EditorStyles.helpBox :
				GUI.skin.box;

			Rect rect =
				EditorGUILayout.BeginVertical(
					style
				);

			if (Event.current.type == EventType.MouseDown &&
				rect.Contains(Event.current.mousePosition))
			{
				SelectAsset(asset);
			}

			EditorGUILayout.BeginHorizontal();

			DrawSprite(
				GetIcon(asset),
				40
			);

			EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField(
				asset.name,
				cardTitleStyle
			);

			EditorGUILayout.LabelField(
				GetSubtitle(asset),
				cardSubtitleStyle
			);

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

			GUILayout.Space(3);
		}
		private void DrawSprite(
			Sprite sprite,
			float size)
		{
			if (sprite == null)
			{
				GUILayout.Label(
					EditorGUIUtility.IconContent(
						"ScriptableObject Icon"
					).image,
					GUILayout.Width(size),
					GUILayout.Height(size)
				);

				return;
			}

			Rect rect =
				sprite.rect;

			Texture texture =
				sprite.texture;

			Rect uv = new Rect(
				rect.x / texture.width,
				rect.y / texture.height,
				rect.width / texture.width,
				rect.height / texture.height
			);

			Rect position =
				GUILayoutUtility.GetRect(
					size,
					size
				);

			GUI.DrawTextureWithTexCoords(
				position,
				texture,
				uv
			);
		}
		private void DrawInspector()
		{
			EditorGUILayout.BeginVertical();

			if (selected == null)
			{
				EditorGUILayout.HelpBox(
					"Select an asset from the list.",
					MessageType.Info
				);

				EditorGUILayout.EndVertical();
				return;
			}

			DrawInspectorHeader();

			inspectorScroll =
				EditorGUILayout.BeginScrollView(
					inspectorScroll
				);

			if (selectedEditor == null)
			{
				selectedEditor =
					Editor.CreateEditor(selected);
			}

			selectedEditor.OnInspectorGUI();

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}

		private void DrawInspectorHeader()
		{
			EditorGUILayout.BeginVertical(
				EditorStyles.helpBox
			);

			EditorGUILayout.BeginHorizontal();

			DrawSprite(
				GetIcon(selected),
				64
			);

			EditorGUILayout.BeginVertical();

			EditorGUILayout.LabelField(
				selected.name,
				titleStyle
			);

			EditorGUILayout.LabelField(
				GetSubtitle(selected),
				subtitleStyle
			);

			if (GUILayout.Button(
				"Select In Project",
				GUILayout.Width(140)
			))
			{
				Selection.activeObject = selected;
				EditorGUIUtility.PingObject(selected);
			}

			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

			GUILayout.Space(4);
		}

		private void SelectAsset(
			ScriptableObject asset)
		{
			selected = asset;

			if (selectedEditor != null)
			{
				DestroyImmediate(selectedEditor);
			}

			selectedEditor =
				Editor.CreateEditor(selected);

			Repaint();
		}

		private Sprite GetIcon(
			ScriptableObject asset)
		{
			switch (asset)
			{
				case SpellModuleDefinition module:

					return module.Icon;

				case SpellBehaviorDefinition behavior:

					return behavior.Icon;
			}

			return null;
		}

		private string GetSubtitle(
			ScriptableObject asset)
		{
			if (asset is SpellModuleDefinition)
			{
				return "Spell Module";
			}

			if (asset is SpellBehaviorDefinition)
			{
				return "Spell Behavior";
			}

			if (asset is StatusDefinition)
			{
				return "Status";
			}

			if (asset is EntityDefinition)
			{
				return "Entity";
			}

			if (asset is AIBehaviorDefinition)
			{
				return "AI Behavior";
			}

			return asset.GetType().Name;
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

			if (selectedEditor != null)
			{
				DestroyImmediate(selectedEditor);
				selectedEditor = null;
			}

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