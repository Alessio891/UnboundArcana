using UnboundArcana.Core.Research;
using UnityEngine;
using UnityEngine.UI;

public class ResearchProgressEntryUI : MonoBehaviour
{
	[SerializeField] private Text progress;
	public ResearchDefinition definition;
	public ResearchInstance researchInstance;
	public void Initialize(ResearchDefinition research) {
		progress.text = $"{research.DisplayName} 0/{research.RequiredKnowledge}";
		this.definition = research ;
	}
	public void UpdateProgress(ResearchInstance instance) {
		researchInstance = instance;
		progress.text = $"{instance.Definition.DisplayName} {instance.Knowledge}/{instance.Definition.RequiredKnowledge}";
		if (instance.Knowledge >= instance.Definition.RequiredKnowledge)
			progress.color = Color.green;
	}
}
