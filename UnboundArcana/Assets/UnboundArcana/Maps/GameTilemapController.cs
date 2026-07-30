using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTilemapController : MonoBehaviour
{
	TilemapRenderer tilemapRenderer;

	private void Awake()
	{
		tilemapRenderer = GetComponentInChildren<TilemapRenderer>();
	}
	private void Start()
	{
		
	}

	public void StartConstructing() {
		StartCoroutine(test());
	}

	public IEnumerator FadeOut() {
		float val = 1.0f;
		while (true)
		{
			val -= 0.8f * Time.deltaTime;
			tilemapRenderer.material.SetFloat("_Progress", val);
			yield return new WaitForEndOfFrame();
			if (val <= 0.0f) break;
		}
		yield return null; 
	}

	IEnumerator test() {
		float val = 0.0f;
		while(true) {
			val += 0.8f * Time.deltaTime;
			tilemapRenderer.material.SetFloat("_Progress", val);
			yield return new WaitForEndOfFrame();
			if (val >= 1.0f) break;
		}

		GameRuntimeManager.Instance.Events.Publish(new MapConstructionEvent(true));
	}
}
