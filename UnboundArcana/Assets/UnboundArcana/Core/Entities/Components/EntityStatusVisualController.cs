using System;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Entities.Statuses;
using UnityEngine;

public class EntityStatusVisualController : MonoBehaviour
{
	private Entity entity;

	private void Awake()
	{
		entity = GetComponent<Entity>();
	}

	private void OnEnable()
	{
		entity?.Events.Subscribe<EntityStatusAppliedEvent>(OnStatusApplied);
		entity?.Events.Subscribe<EntityStatusRemovedEvent>(OnStatusRemoved);
	}
	private void OnDisable()
	{
		entity?.Events.Unsubscribe<EntityStatusAppliedEvent>(OnStatusApplied);
		entity?.Events.Unsubscribe<EntityStatusRemovedEvent>(OnStatusRemoved);
	}

	private void OnStatusRemoved(EntityStatusRemovedEvent evt)
	{
		if (evt.StatusInstance.Definition is ChilledStatusDefinition) {
			GetComponentInChildren<SpriteRenderer>().color = Color.white;
		}
	}

	private void OnStatusApplied(EntityStatusAppliedEvent evt)
	{
		if (evt.StatusInstance.Definition is ChilledStatusDefinition)
		{
			GetComponentInChildren<SpriteRenderer>().color = Color.blue;
		}
	}

}
