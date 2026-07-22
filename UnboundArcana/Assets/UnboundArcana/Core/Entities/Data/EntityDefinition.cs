using UnboundArcana.Core.Entities.AI;
using UnboundArcana.Spells.Data;
using UnityEngine;
using System.Collections.Generic;

namespace UnboundArcana.Core.Entities
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Entity Definition"
	)]
	public class EntityDefinition : ScriptableObject
	{
		[SerializeField]
		private GameObject prefab;

		public float maxHealth = 100f;
		public float moveSpeed = 5f;
		public float castSpeed = 1f;
		public float armor = 0f;

		public AIBehaviorDefinition behavior;

		public List<SpellDefinition> initialSpells;

		public GameObject Prefab => prefab;
	}
}