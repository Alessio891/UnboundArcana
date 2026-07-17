
using UnityEngine;
using UnityEngine.UI;

namespace UnboundArcana.UI
{
	public class InteractionPrompt : MonoBehaviour
	{
		[SerializeField]
		private Text text;

		public void SetText(string value)
		{
			text.text = value;
		}
	}
}