using TMPro;
using UnityEngine;

public class DamageTextView : MonoBehaviour
{
	[SerializeField]
	private TMP_Text text;

	public void Initialize(
		Vector3 position,
		float damage)
	{
		transform.position = position;

		text.text = Mathf.RoundToInt(damage).ToString();

		iTween.MoveBy(
			gameObject,
			iTween.Hash(
				"y", 1.3f,
				"time", 0.9f,
				"easetype", iTween.EaseType.easeOutQuad
			)
		);

		iTween.ValueTo(
			gameObject,
			iTween.Hash(
				"from", 1f,
				"to", 0f,
				"time", 1.2f,
				"onupdate", "SetAlpha",
				"oncomplete", "Finish"
			)
		);
	}

	private void SetAlpha(float alpha)
	{
		Color c = text.color;
		c.a = alpha;
		text.color = c;
	}

	private void Finish()
	{
		Destroy(gameObject);
	}
}