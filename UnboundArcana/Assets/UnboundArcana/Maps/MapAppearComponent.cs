using System;
using UnityEngine;


public class MapAppearComponent : MonoBehaviour
{
	[SerializeField]
	private float appearDelay = 0.0f;

	[SerializeField] 
	private float appearTime = 0.6f;
	private void Awake()
	{
		GetComponentInChildren<SpriteRenderer>().material.SetFloat("_Progress", 0.0f);
	}
	private void OnEnable()
	{
		GameRuntimeManager.Instance.Events.Subscribe<MapConstructionEvent>(OnMapConstructed);
	}
	private void OnDisable()
	{
		GameRuntimeManager.Instance.Events.Unsubscribe<MapConstructionEvent>(OnMapConstructed);
	}

	private void OnMapConstructed(MapConstructionEvent evt)
	{
		float delay = UnityEngine.Random.Range(0.0f, 0.6f);
		if (evt.IsConstructing)
			iTween.ValueTo(gameObject, iTween.Hash("from", 0.0f, "to", 1.0f, "time", appearTime, "delay", delay, "onupdate", "AnimationStateUpdate"));
		else
			iTween.ValueTo(gameObject, iTween.Hash("from", 1.0f, "to", 0.0f, "time", appearTime, "delay", delay, "onupdate", "AnimationStateUpdate"));
	}
	void AnimationStateUpdate(float value) {
		GetComponentInChildren<SpriteRenderer>().material.SetFloat("_Progress", value);
	}
}
