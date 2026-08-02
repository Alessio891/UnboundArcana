using UnboundArcana.Core.Research;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchProgressEntryUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	[SerializeField] private Text progress;
	[SerializeField] private Image progressBar;
	public ResearchDefinition definition;
	public void Initialize(ResearchDefinition research) {
		progress.text = research.DisplayName;
		progressBar.transform.parent.gameObject.SetActive(false);
		this.definition = research ;
	}
	public void OnPointerEnter(PointerEventData eventData) {
		if (definition != null) progress.text = $"<size=11>{definition.Description}</size>";
	}
	public void OnPointerExit(PointerEventData eventData) {
		if (definition != null) progress.text = definition.DisplayName;
	}
}
