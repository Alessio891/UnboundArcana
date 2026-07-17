using UnityEngine;

namespace UnboundArcana.Core.Entities.AI
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/AI Profile"
	)]
	public class AIProfile : ScriptableObject
	{
		[SerializeField]
		private float detectionRange = 5f;

		[SerializeField]
		private float attackRange = 2f;

		[SerializeField]
		private float attackCooldown = 1f;


		public float DetectionRange => detectionRange;

		public float AttackRange => attackRange;

		public float AttackCooldown => attackCooldown;
	}
}