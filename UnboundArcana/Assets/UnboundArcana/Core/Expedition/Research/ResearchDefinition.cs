using UnboundArcana.Core.Runtime;
using UnityEngine;

namespace UnboundArcana.Core.Research
{
	[CreateAssetMenu(
		menuName = "Unbound Arcana/Research/Research Definition")]
	public class ResearchDefinition : ScriptableObject
	{
		[SerializeField]
		private string researchId;

		[SerializeField]
		private string displayName;

		[TextArea]
		[SerializeField]
		private string description;

		[SerializeField]
		private int requiredKnowledge = 100;

		[SerializeField]
		private RunModifierStat modifierStat;

		[SerializeField]
		private RunModifierOperation modifierOperation;

		[SerializeField]
		private float modifierValue;

		public string ResearchId => researchId;
		public string DisplayName => displayName;
		public string Description => description;
		public int RequiredKnowledge => requiredKnowledge;
		public RunModifierStat ModifierStat => modifierStat;
		public RunModifierOperation ModifierOperation => modifierOperation;
		public float ModifierValue => modifierValue;
	}
}