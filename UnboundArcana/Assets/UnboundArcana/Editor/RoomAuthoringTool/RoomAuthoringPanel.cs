using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnboundArcana.Core.Rooms.Editor
{
	public class RoomAuthoringPanel : IDisposable
	{
		public VisualElement Root { get; private set; }

		private DropdownField sceneDropdown;
		private DropdownField prefabDropdown;
		private Label prefabSourceLabel;
		private Label statsLabel;

		public RoomAuthoringPanel()
		{
			CreateUI();

			Subscribe();

			Refresh();
		}


		private void CreateUI()
		{
			Root =
				new VisualElement();

			Root.style.paddingLeft = 8;
			Root.style.paddingRight = 8;
			Root.style.paddingTop = 8;
			Root.style.paddingBottom = 8;


			ScrollView scroll =
				new ScrollView();

			Root.Add(scroll);


			scroll.Add(
				new Label("Room Authoring"));


			scroll.Add(
				CreateSpacer());


			sceneDropdown =
				CreateSceneDropdown();

			scroll.Add(sceneDropdown);


			scroll.Add(
				CreateSceneControls());


			scroll.Add(
				CreateSpacer());


			prefabDropdown =
				CreatePrefabDropdown();

			scroll.Add(prefabDropdown);

			scroll.Add(
	CreatePrefabControls());
			Button spawn =
				new Button();

			spawn.text =
				"Spawn Prefab Section";

			spawn.clicked += () =>
			{
				RoomSectionPlacementUtility.Spawn(
					RoomSectionDatabase.SelectedPrefabSection);

				Refresh();
			};

			scroll.Add(spawn);


			scroll.Add(
				CreateSpacer());
			scroll.Add(
CreateConnectorTools());

			Label statsTitle =
				new Label(
					"Selected Section Stats");

			statsTitle.style.unityFontStyleAndWeight =
				FontStyle.Bold;

			scroll.Add(statsTitle);


			statsLabel =
				new Label();

			scroll.Add(statsLabel);
		}
		private VisualElement CreateConnectorTools()
		{
			VisualElement container =
				new VisualElement();


			Label title =
				new Label(
					"Connector Tools");


			title.style.unityFontStyleAndWeight =
				FontStyle.Bold;


			container.Add(title);


			RoomSection section =
				RoomSectionDatabase.SelectedSceneSection;


			if (section == null)
			{
				container.Add(
					new Label(
						"No section selected"));

				return container;
			}


			foreach (RoomSectionConnector connector in section.Connectors)
			{
				RoomSectionConnector current =
					connector;


				Button button =
					new Button();


				button.text =
					current.name;


				button.clicked += () =>
				{
					RoomSectionConnectorUtility.Select(
						current);
				};


				container.Add(button);
			}


			Button add =
				new Button();


			add.text =
				"Add Connector";


			add.clicked += () =>
			{
				RoomSectionConnector connector =
					RoomSectionConnectorUtility.CreateConnector(
						section);

			};


			container.Add(add);


			return container;
		}
		private VisualElement CreatePrefabControls()
		{
			VisualElement container =
				new VisualElement();


			Label title =
				new Label(
					"Prefab Management");

			title.style.unityFontStyleAndWeight =
				FontStyle.Bold;

			container.Add(title);


			prefabSourceLabel =
				new Label();

			container.Add(
				prefabSourceLabel);


			Button create =
				new Button();

			create.text =
				"Create Prefab";

			create.clicked += () =>
			{
				RoomSectionPrefabUtility.CreatePrefab(
					RoomSectionDatabase.SelectedSceneSection);

				Refresh();
			};

			container.Add(create);


			Button apply =
				new Button();

			apply.text =
				"Apply Changes";

			apply.clicked += () =>
			{
				RoomSectionPrefabUtility.ApplyChanges(
					RoomSectionDatabase.SelectedSceneSection);

				Refresh();
			};

			container.Add(apply);


			Button ping =
				new Button();

			ping.text =
				"Ping Prefab";

			ping.clicked += () =>
			{
				RoomSectionPrefabUtility.PingPrefab(
					RoomSectionDatabase.SelectedSceneSection);
			};

			container.Add(ping);


			return container;
		}
		private DropdownField CreateSceneDropdown()
		{
			DropdownField dropdown =
				new DropdownField(
					"Scene Section",
					new List<string>(),
					0);


			dropdown.RegisterValueChangedCallback(evt =>
			{
				foreach (RoomSection section in
					RoomSectionDatabase.SceneSections)
				{
					if (section.SectionId != evt.newValue)
						continue;


					RoomSectionDatabase.SelectedSceneSection =
						section;


					RoomSectionSelectionUtility.Select(
						section);


					Refresh();

					break;
				}
			});


			return dropdown;
		}


		private DropdownField CreatePrefabDropdown()
		{
			DropdownField dropdown =
				new DropdownField(
					"Prefab Section",
					new List<string>(),
					0);


			dropdown.RegisterValueChangedCallback(evt =>
			{
				foreach (RoomSection section in
					RoomSectionDatabase.PrefabSections)
				{
					if (section.SectionId != evt.newValue)
						continue;


					RoomSectionDatabase.SelectedPrefabSection =
						section;

					break;
				}
			});


			return dropdown;
		}


		private VisualElement CreateSceneControls()
		{
			VisualElement container =
				new VisualElement();


			Button select =
				new Button();

			select.text =
				"Select";

			select.clicked += () =>
			{
				RoomSectionSelectionUtility.Select(
					RoomSectionDatabase.SelectedSceneSection);
			};

			container.Add(select);


			Button frame =
				new Button();

			frame.text =
				"Frame";

			frame.clicked += () =>
			{
				RoomSectionSelectionUtility.Frame(
					RoomSectionDatabase.SelectedSceneSection);
			};

			container.Add(frame);


			Button enable =
				new Button();

			enable.text =
				"Enable";

			enable.clicked += () =>
			{
				RoomSectionSelectionUtility.SetEnabled(
					RoomSectionDatabase.SelectedSceneSection,
					true);
			};

			container.Add(enable);


			Button disable =
				new Button();

			disable.text =
				"Disable";


			disable.clicked += () =>
			{
				RoomSectionSelectionUtility.SetEnabled(
					RoomSectionDatabase.SelectedSceneSection,
					false);
			};

			container.Add(disable);


			return container;
		}


		private void Subscribe()
		{
			EditorApplication.hierarchyChanged += Refresh;

			EditorApplication.projectChanged += Refresh;

			Selection.selectionChanged += Refresh;
		}


		private void Unsubscribe()
		{
			EditorApplication.hierarchyChanged -= Refresh;

			EditorApplication.projectChanged -= Refresh;

			Selection.selectionChanged -= Refresh;
		}


		private void Refresh()
		{
			if (sceneDropdown == null)
				return;


			RefreshSceneDropdown();

			RefreshPrefabDropdown();

			RefreshStats();

			RefreshPrefabInfo();
		}
		private void RefreshPrefabInfo()
		{
			if (prefabSourceLabel == null)
				return;

			RoomSection section =
				RoomSectionDatabase.SelectedSceneSection;

			if (section == null)
			{
				prefabSourceLabel.text =
					"Prefab Source: None";

				return;
			}

			string path =
				RoomSectionPrefabUtility.GetPrefabPath(
					section);

			if (string.IsNullOrEmpty(path))
			{
				prefabSourceLabel.text =
					"Prefab Source: None";

				return;
			}

			string prefabName =
				System.IO.Path.GetFileNameWithoutExtension(
					path);

			prefabSourceLabel.text =
				$"Prefab Source: {prefabName}";
		}

		private void RefreshSceneDropdown()
		{
			List<string> names =
				new();


			foreach (RoomSection section in
				RoomSectionDatabase.SceneSections)
			{
				names.Add(
					section.SectionId);
			}


			sceneDropdown.choices =
				names;


			RoomSection selected =
				RoomSectionDatabase.SelectedSceneSection;


			if (selected != null)
			{
				sceneDropdown.SetValueWithoutNotify(
					selected.SectionId);
			}
		}


		private void RefreshPrefabDropdown()
		{
			List<string> names =
				new();


			foreach (RoomSection section in
				RoomSectionDatabase.PrefabSections)
			{
				names.Add(
					section.SectionId);
			}


			prefabDropdown.choices =
				names;


			RoomSection selected =
				RoomSectionDatabase.SelectedPrefabSection;


			if (selected != null)
			{
				prefabDropdown.SetValueWithoutNotify(
					selected.SectionId);
			}
		}


		private void RefreshStats()
		{
			RoomSection section =
				RoomSectionDatabase.SelectedSceneSection;


			if (section == null)
			{
				statsLabel.text =
					"No section selected";

				return;
			}


			statsLabel.text =
				$"ID: {section.SectionId}\n" +
				$"Footprint: {RoomSectionStatsUtility.GetFootprintSize(section)}\n" +
				$"Connectors: {RoomSectionStatsUtility.GetConnectorCount(section)}\n" +
				$"Markers: {RoomSectionStatsUtility.GetMarkerCount(section)}\n" +
				$"Tilemaps: {RoomSectionStatsUtility.GetTilemapCount(section)}\n" +
				$"Tiles: {RoomSectionStatsUtility.GetTileCount(section)}";
		}


		private VisualElement CreateSpacer()
		{
			return new VisualElement
			{
				style =
				{
					height = 8
				}
			};
		}


		public void Dispose()
		{
			Unsubscribe();
		}
	}
}