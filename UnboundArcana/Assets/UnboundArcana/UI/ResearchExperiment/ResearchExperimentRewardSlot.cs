using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResearchExperimentRewardSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	[SerializeField] private Image icon;
	[SerializeField] private Text cost;
	public ReserachExperimentUI uiManager;
	GameReward reward;

	public void OnBeginDrag(PointerEventData eventData)
	{
		uiManager.StartDragReward(reward);
	}

	public void OnDrag(PointerEventData eventData)
	{
		uiManager.UpdateDragReward(eventData);
	}

	public void OnDrop(PointerEventData eventData)
	{
		uiManager.EndDragReward();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		uiManager.EndDragReward();
	}

	public void SetReward(GameReward reward)
	{
		this.reward = reward;

		icon.sprite = reward.icon;
		cost.text = reward.cost.ToString();
	}
	
}
