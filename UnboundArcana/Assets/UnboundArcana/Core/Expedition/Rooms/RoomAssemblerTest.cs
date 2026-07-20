using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class RoomAssemblerTester : MonoBehaviour
	{
		[SerializeField]
		private List<RoomSection> sections = new();

		[SerializeField]
		private int sectionCount = 4;

		private readonly List<GameObject> spawnedSections = new();

		private Dictionary<GeneratedSection, RoomSection> instances;

		private void Start()
		{
			StartCoroutine(loopGenerate());
		}

		IEnumerator loopGenerate()
		{
			while(true) {
				GenerateRoom();
				yield return new WaitForSeconds(0.5f);
			}
		}

		[ContextMenu("Generate Room")]
		public void GenerateRoom()
		{
			Clear();

			var assembler = new RoomAssembler(sections);

			var layout = assembler.Generate(sectionCount);

			instances = new();

			foreach (var section in layout.Sections)
			{
				var instance = Instantiate(
					section.Template,
					new Vector3(
						section.CellPosition.x * 0.3f,
						section.CellPosition.y * 0.3f,
						0),
					Quaternion.identity,
					transform);

				spawnedSections.Add(instance.gameObject);

				instances.Add(section, instance);
			}

			foreach (var connection in layout.Connections)
			{
				var a =
					instances[connection.A]
					.Connectors[connection.ConnectorAIndex];

				var b =
					instances[connection.B]
					.Connectors[connection.ConnectorBIndex];

				a.ConnectedTo = b;
				b.ConnectedTo = a;
			}
		}

		[ContextMenu("Clear Room")]
		public void Clear()
		{
			foreach (var section in spawnedSections)
			{
				if (section != null)
					DestroyImmediate(section);
			}

			spawnedSections.Clear();
		}
	}
}