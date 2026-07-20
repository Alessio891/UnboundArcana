using UnboundArcana.Core.Entities;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	[SerializeField] private bool isTutorial = false;
	[Multiline(3)]
	[SerializeField] 
	private string message;
	[SerializeField]
	private bool destroyAfterTrigger = true;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<Entity>() == null) return;

		if (isTutorial)
		{
			bool showTutorial = SettingsManager.Instance.Get<bool>("show_tutorial");
			if (!showTutorial) { return; }
		}

		GameRuntimeManager.Instance.Events.Publish(new ShowDialogueEvent(message, null));
		if (destroyAfterTrigger) Destroy(gameObject);
	}


}
