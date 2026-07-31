#if UNITY_EDITOR
using UnboundArcana.Core.Entities;
using UnboundArcana.Player;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Modules;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnboundArcana.Sandbox.Debugging
{
	public class SpellRuntimeDebugPanel : MonoBehaviour
	{
		private const string CatalogPath = "Assets/UnboundArcana/Data/SpellDataCatalog.asset";
		private Rect windowRect = new Rect(12f, 12f, 300f, 460f);
		private Vector2 scroll;
		private SpellDataCatalog catalog;
		private SpellCaster caster;
		private bool visible = true;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Create()
		{
			if (!Application.isEditor || FindFirstObjectByType<SpellRuntimeDebugPanel>() != null) { return; }

			GameObject instance = new GameObject("Spell Runtime Debug Panel");
			DontDestroyOnLoad(instance);
			instance.AddComponent<SpellRuntimeDebugPanel>();
		}

		private void Awake()
		{
			catalog = AssetDatabase.LoadAssetAtPath<SpellDataCatalog>(CatalogPath);
		}

		private void Update()
		{
			if (Keyboard.current?.f8Key.wasPressedThisFrame == true) { visible = !visible; }

			if (caster == null)
			{
				PlayerController player = FindFirstObjectByType<PlayerController>();
				if (player != null) { caster = player.GetComponent<SpellCaster>(); }
			}
		}

		private void OnGUI()
		{
			if (!visible) { return; }

			windowRect = GUILayout.Window(GetInstanceID(), windowRect, DrawWindow, "Spell Runtime Debug");
		}

		private void DrawWindow(int id)
		{
			if (catalog == null)
			{
				GUILayout.Label("SpellDataCatalog not found.");
				GUI.DragWindow();
				return;
			}

			SpellConfiguration configuration = GetConfiguration();
			if (configuration == null)
			{
				GUILayout.Label("No initialized SpellCaster found.");
				if (GUILayout.Button("Refresh")) { caster = FindFirstObjectByType<SpellCaster>(); }
				GUI.DragWindow();
				return;
			}

			GUILayout.Label("Next cast | F8 hide", EditorStyles.miniLabel);
			scroll = GUILayout.BeginScrollView(scroll);
			DrawBehaviors(configuration);
			GUILayout.Space(4f);
			DrawModules(configuration);
			GUILayout.EndScrollView();
			GUI.DragWindow(new Rect(0f, 0f, windowRect.width, 24f));
		}

		private SpellConfiguration GetConfiguration()
		{
			if (caster == null || caster.SpellLoadout == null) { return null; }

			return caster.SpellLoadout.GetCurrentSpell()?.Configuration;
		}

		private void DrawBehaviors(SpellConfiguration configuration)
		{
			GUILayout.Label("Behavior", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();

			foreach (var behavior in catalog.behaviors)
			{
				if (behavior == null) { continue; }

				bool selected = configuration.behavior == behavior;
				GUI.enabled = !selected;
				if (GUILayout.Button(selected ? $"[{behavior.name}]" : behavior.name, GUILayout.Height(22f)))
				{
					RemoveIncompatibleModules(configuration, behavior);
					configuration.SetBehavior(behavior);
				}
				GUI.enabled = true;
			}

			GUILayout.EndHorizontal();
		}

		private void DrawModules(SpellConfiguration configuration)
		{
			GUILayout.Label("Modules", EditorStyles.boldLabel);

			for (int i = 0; i < catalog.modules.Count; i += 2)
			{
				GUILayout.BeginHorizontal();
				DrawModuleButton(configuration, catalog.modules[i]);
				if (i + 1 < catalog.modules.Count) { DrawModuleButton(configuration, catalog.modules[i + 1]); }
				else { GUILayout.FlexibleSpace(); }
				GUILayout.EndHorizontal();
			}
		}

		private void DrawModuleButton(SpellConfiguration configuration, SpellModuleDefinition module)
		{
			if (module == null) { return; }

			bool selected = configuration.HasModule(module);
			bool compatible = module.SupportsBehavior(configuration.behavior);
			GUI.enabled = selected || compatible;
			if (GUILayout.Button(selected ? $"[-] {module.name}" : module.name, GUILayout.MinWidth(128f), GUILayout.Height(22f)))
			{
				ToggleModule(configuration, module, selected);
			}
			GUI.enabled = true;
		}

		private void ToggleModule(SpellConfiguration configuration, SpellModuleDefinition module, bool selected)
		{
			if (selected)
			{
				configuration.RemoveModule(module);
				return;
			}

			if (module.Type == SpellModuleType.Principle)
			{
				for (int i = configuration.modules.Count - 1; i >= 0; i--)
				{
					if (configuration.modules[i] != null && configuration.modules[i].Type == SpellModuleType.Principle)
					{
						configuration.RemoveModule(configuration.modules[i]);
					}
				}
			}

			configuration.AddModule(module);
		}

		private void RemoveIncompatibleModules(SpellConfiguration configuration, UnboundArcana.Spells.Behaviors.SpellBehaviorDefinition behavior)
		{
			for (int i = configuration.modules.Count - 1; i >= 0; i--)
			{
				SpellModuleDefinition module = configuration.modules[i];
				if (module != null && !module.SupportsBehavior(behavior)) { configuration.RemoveModule(module); }
			}
		}
	}
}
#endif
