using UnityEngine;
using UnityEngine.UI;

public class MainMenuOption : MonoBehaviour
{
	public Image[] Cursors;
	public Image Highlight;

	private void Awake()
	{
		if (Highlight != null) { Highlight.enabled = false; }
		foreach (Image img in Cursors)
		{
			if (img != null)
			{
				img.enabled = false;
			}
		}
	}

	public void SetHighlighted(bool highlighted) {
		if (Highlight != null)
			Highlight.enabled = highlighted;

		foreach (Image img in Cursors)
		{
			if (img != null)
			{
				img.enabled = highlighted;
			}
		}
	}
}
