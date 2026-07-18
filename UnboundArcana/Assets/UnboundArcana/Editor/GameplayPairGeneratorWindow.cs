using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using UnityEngine.WSA;

namespace UnboundArcana.EditorTools
{
	public class GameplayPairGeneratorWindow : EditorWindow
	{
		private const string PendingAssetKey =
		"UA_PendingDefinitionAsset";
		private enum GeneratorType
		{
			SpellModule,
			SpellBehavior,
			Status,
			AI
		}
		private bool useSelectedFolder = true;
		private GeneratorType type;
		private string className = "";
		private DefaultAsset outputFolder;

		[MenuItem("Tools/Unbound Arcana/Gameplay Pair Generator")]
		private static void Open()
		{
			GetWindow<GameplayPairGeneratorWindow>(
				"Gameplay Generator"
			);
		}

		private void OnGUI()
		{
			type = (GeneratorType)EditorGUILayout.EnumPopup(
				"Type",
				type
			);

			className = EditorGUILayout.TextField(
				"Name",
				className
			);

			useSelectedFolder = EditorGUILayout.Toggle(
	"Use Selected Folder",
	useSelectedFolder
);

			if (!useSelectedFolder)
			{
				outputFolder = (DefaultAsset)EditorGUILayout.ObjectField(
					"Output Folder",
					outputFolder,
					typeof(DefaultAsset),
					false
				);
			}

			GUI.enabled =
				!string.IsNullOrWhiteSpace(className) &&
				(useSelectedFolder || outputFolder != null);

			if (GUILayout.Button("Create"))
			{
				Generate();
			}
			if (GUILayout.Button("Create Asset")) {
				string folder = AssetDatabase.GetAssetPath(outputFolder);
				CreatePendingAsset(
							 folder,
							 className,
							 type
						 );
			}
			GUI.enabled = true;
		}
		private void CreatePendingAsset(
		string folder,
		string name,
		GeneratorType type)
		{
			string definitionName = "";

			switch (type)
			{
				case GeneratorType.SpellModule:
					definitionName =
						name + "ModuleDefinition";
					break;

				case GeneratorType.SpellBehavior:
					definitionName =
						name + "BehaviorDefinition";
					break;

				case GeneratorType.Status:
					definitionName =
						name + "StatusDefinition";
					break;
				case GeneratorType.AI:
					definitionName = name + "AIDefinition";
					break;
			}

			string data =
				$"{folder}|{definitionName}";

			EditorPrefs.SetString(
				PendingAssetKey,
				data
			);
		}
		private void Generate()
		{
			string folder =
	useSelectedFolder
		? GetSelectedFolder()
		: AssetDatabase.GetAssetPath(outputFolder);

			if (string.IsNullOrEmpty(folder))
			{
				Debug.LogError(
					"No valid output folder selected."
				);

				return;
			}

			switch (type)
			{
				case GeneratorType.SpellModule:
					CreateSpellModule(folder);
					break;

				case GeneratorType.SpellBehavior:
					CreateSpellBehavior(folder);
					break;

				case GeneratorType.Status:
					CreateStatus(folder);
					break;
				case GeneratorType.AI: CreateAI(folder); break;
			}
			AssetDatabase.Refresh();
			
		}
		private string GetSelectedFolder()
		{
			Object selected = Selection.activeObject;

			if (selected == null)
			{
				return null;
			}

			string path =
				AssetDatabase.GetAssetPath(selected);

			if (AssetDatabase.IsValidFolder(path))
			{
				return path;
			}

			return Path.GetDirectoryName(path);
		}
		private void CreateSpellModule(string folder)
		{
			string definition =
$@"using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Spells.Modules.{className}
{{
	[CreateAssetMenu(menuName = ""Spells/Modules/{className}"")]
	public class {className}ModuleDefinition : SpellModuleDefinition
	{{
		public override SpellModule CreateRuntime()
		{{
			return new {className}Module(this);
		}}
	}}
}}";

			string runtime =
$@"using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Modules.{className}
{{
	public class {className}Module : SpellModule
	{{
		private readonly {className}ModuleDefinition definition;

		public {className}Module(
			{className}ModuleDefinition definition)
		{{
			this.definition = definition;
		}}
	}}
}}";

			WriteFile(
				folder,
				$"{className}ModuleDefinition.cs",
				definition
			);

			WriteFile(
				folder,
				$"{className}Module.cs",
				runtime
			);
		}

		private void CreateAI(string folder) {
			string definition =
	$@"using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{{
	[CreateAssetMenu(menuName = ""SUnbound Arcana/AI/{className}"")]
	public class {className}Definition : AIBehaviorDefinition
	{{
		public override AIBehavior CreateBehavior()
		{{
			return new {className}();
		}}
	}}
}}";

			string runtime =
$@"using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{{
	public class {className} : AIBehavior
	{{
		protected override void OnInitialize()
		{{
		}}
		protected override void OnTick()
		{{
		}}
	}}
}}";

			WriteFile(
				folder,
				$"{className}Definition.cs",
				definition
			);

			WriteFile(
				folder,
				$"{className}.cs",
				runtime
			);
		}

		private void CreateSpellBehavior(string folder)
		{
			string definition =
$@"using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Behaviors.{className}
{{
	[CreateAssetMenu(menuName = ""Spells/Behaviors/{className}"")]
	public class {className}BehaviorDefinition : SpellBehaviorDefinition
	{{
		public override SpellBehavior CreateRuntime()
		{{
			return new {className}Behavior();
		}}
	}}
}}";

			string runtime =
$@"using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Spells.Behaviors.{className}
{{
	public class {className}Behavior : SpellBehavior
	{{
		public override void Cast(
			CastContext context)
		{{
		}}
	}}
}}";

			WriteFile(
				folder,
				$"{className}BehaviorDefinition.cs",
				definition
			);

			WriteFile(
				folder,
				$"{className}Behavior.cs",
				runtime
			);
		}

		private void CreateStatus(string folder)
		{
			string definition =
$@"using UnityEngine;
using UnboundArcana.Core.Entities.Statuses;

namespace UnboundArcana.Core.Entities.Statuses
{{
	[CreateAssetMenu(menuName = ""Unbound Arcana/Statuses/{className}"")]
	public class {className}StatusDefinition : StatusDefinition
	{{
		public override StatusInstance CreateRuntime()
		{{
			return new {className}Status(this);
		}}
	}}
}}";

			string runtime =
$@"namespace UnboundArcana.Core.Entities.Statuses
{{
	public class {className}Status : StatusInstance
	{{
		public {className}Status(
			StatusDefinition definition)
			: base(definition)
		{{
		}}
	}}
}}";

			WriteFile(
				folder,
				$"{className}StatusDefinition.cs",
				definition
			);

			WriteFile(
				folder,
				$"{className}Status.cs",
				runtime
			);
		}

		private void WriteFile(
			string folder,
			string fileName,
			string content)
		{
			string path =
				Path.Combine(
					folder,
					fileName
				);

			if (File.Exists(path))
			{
				Debug.LogError(
					$"File already exists: {path}"
				);

				return;
			}

			File.WriteAllText(
				path,
				content
			);
		}

		[InitializeOnLoadMethod]
		private static void InitializeAssetCreation()
		{
			EditorApplication.delayCall += TryCreatePendingAsset;
		}
		private static void TryCreatePendingAsset()
		{
			if (!EditorPrefs.HasKey(PendingAssetKey))
			{
				return;
			}

			string data =
				EditorPrefs.GetString(
					PendingAssetKey
				);

			EditorPrefs.DeleteKey(
				PendingAssetKey
			);

			string[] split =
				data.Split('|');

			string folder = split[0];
			string typeName = split[1];

			System.Type type =
				System.Type.GetType(
					typeName
				);

			if (type == null)
			{
				foreach (var assembly in
					System.AppDomain.CurrentDomain.GetAssemblies())
				{
					type =
						assembly.GetType(
							typeName
						);

					if (type != null)
					{
						break;
					}
				}
			}

			if (type == null)
			{
				Debug.LogError(
					$"Could not find {typeName}"
				);

				return;
			}

			ScriptableObject asset =
				ScriptableObject.CreateInstance(
					type
				);

			string path =
				AssetDatabase.GenerateUniqueAssetPath(
					$"{folder}/{typeName}.asset"
				);

			AssetDatabase.CreateAsset(
				asset,
				path
			);

			AssetDatabase.SaveAssets();

			EditorGUIUtility.PingObject(
				asset
			);
		}
	}
}