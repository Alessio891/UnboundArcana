using System.Collections;
using UnityEngine;

public class ShowDialogueEvent
{
	private string message;
	private GameObject source;
	public string Message => message;
	public GameObject Source => source;

	public ShowDialogueEvent(string message, GameObject source) {

		this.message = message;
		this.source = source;
	}


}
