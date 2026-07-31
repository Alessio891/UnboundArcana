using System.Collections;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Principles
{
	public class LightningArcFeedback : MonoBehaviour
	{
		public static void Spawn(Vector3 start, Vector3 end)
		{
			GameObject instance = new GameObject("Lightning Arc");
			instance.AddComponent<LightningArcFeedback>().Initialize(start, end);
		}

		private void Initialize(Vector3 start, Vector3 end)
		{
			LineRenderer line = gameObject.AddComponent<LineRenderer>();
			line.material = new Material(Shader.Find("Sprites/Default"));
			line.startColor = new Color(0.65f, 0.9f, 1f, 1f);
			line.endColor = Color.white;
			line.startWidth = 0.035f;
			line.endWidth = 0.015f;
			line.positionCount = 5;
			line.sortingOrder = 20;

			Vector3 delta = end - start;
			Vector3 normal = new Vector3(-delta.y, delta.x, 0f).normalized;
			line.SetPosition(0, start);
			line.SetPosition(1, Vector3.Lerp(start, end, 0.25f) + normal * 0.06f);
			line.SetPosition(2, Vector3.Lerp(start, end, 0.5f) - normal * 0.04f);
			line.SetPosition(3, Vector3.Lerp(start, end, 0.75f) + normal * 0.05f);
			line.SetPosition(4, end);
			StartCoroutine(Fade(line));
		}

		private IEnumerator Fade(LineRenderer line)
		{
			float duration = 0.16f;
			float elapsed = 0f;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float alpha = 1f - elapsed / duration;
				Color startColor = line.startColor;
				Color endColor = line.endColor;
				startColor.a = alpha;
				endColor.a = alpha;
				line.startColor = startColor;
				line.endColor = endColor;
				yield return null;
			}

			Destroy(line.material);
			Destroy(gameObject);
		}
	}
}
