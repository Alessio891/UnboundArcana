using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuOption : MonoBehaviour
{
	public Image[] Cursors;
	public Image Highlight;

	private Color targetColor;

	private void Awake()
	{
		if (Highlight != null) { Highlight.enabled = true; targetColor = Highlight.color; UpdateHighlight(0.0f); }
		
		foreach (Image img in Cursors)
		{
			if (img != null)
			{
				img.enabled = false;
			}
		}
	}
	public void Clicked() {
		iTween.Stop(Highlight.gameObject);
		iTween.PunchScale(Highlight.gameObject, new Vector3(.1f, .1f, .1f), 0.8f);
	}
	void UpdateHighlight(float value) {
		if (Highlight != null)
		{
			Color c = Highlight.color;
			c.a = value;
			Highlight.color = c;

			Color c2 = Highlight.GetComponent<Outline>().effectColor;
			c2.a = value;
			Highlight.GetComponent<Outline>().effectColor = c2;
		}
	}
	public void SetHighlighted(bool highlighted) {
	

		float start = highlighted ? 0 : Highlight.color.a;
		float to = highlighted ? 1 : 0;
		
		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", to, "time", (highlighted ? 0.5f : 0.2f), "onupdate", "UpdateHighlight"));

		foreach (Image img in Cursors)
		{
			if (img != null)
			{
				img.enabled = highlighted;
			}
		}
	}
}
