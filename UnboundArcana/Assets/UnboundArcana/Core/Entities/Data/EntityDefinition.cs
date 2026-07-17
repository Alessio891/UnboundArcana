using UnboundArcana.Core.Entities.AI;
using UnityEngine;

namespace UnboundArcana.Core.Entities
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Entity Definition"
	)]
	public class EntityDefinition : ScriptableObject
	{
		public float maxHealth = 100f;
		public float moveSpeed = 5f;
		public float castSpeed = 1f;
		public float armor = 0f;
		public AIProfile aiProfile;
		public AIBehaviorDefinition behavior;
	}
}