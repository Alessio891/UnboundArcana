using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class RadialLayoutGroup : LayoutGroup
{
	[SerializeField] private float radius = 100f;
	[SerializeField] private float startAngle = 0f;
	[SerializeField] private float arc = 360f;
	[SerializeField] private bool clockwise = true;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
	}

	public override void CalculateLayoutInputVertical()
	{
	}

	public override void SetLayoutHorizontal()
	{
		ArrangeChildren();
	}

	public override void SetLayoutVertical()
	{
		ArrangeChildren();
	}

	private void ArrangeChildren()
	{
		int count = rectChildren.Count;

		if (count == 0)
			return;

		float angleStep = count == 1
			? 0
			: arc / (arc >= 360f ? count : count - 1);

		for (int i = 0; i < count; i++)
		{
			RectTransform child = rectChildren[i];

			float angle = startAngle + (clockwise ? -1 : 1) * angleStep * i;
			float radians = angle * Mathf.Deg2Rad;

			Vector2 position = new Vector2(
				Mathf.Cos(radians),
				Mathf.Sin(radians)
			) * radius;

			SetChildAlongAxis(child, 0, position.x - child.rect.width * child.pivot.x);
			SetChildAlongAxis(child, 1, -position.y - child.rect.height * (1f - child.pivot.y));
		}
	}

#if UNITY_EDITOR
	protected override void OnValidate()
	{
		base.OnValidate();
		SetDirty();
	}
#endif
}