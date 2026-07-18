using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Entities.Statuses
{
	public class StatusController : MonoBehaviour
	{
		private Entity entity;

		private readonly List<StatusInstance> statuses = new();

		private void Awake()
		{
			entity = GetComponent<Entity>();
		}

		private void Update()
		{
			for (int i = statuses.Count - 1; i >= 0; i--)
			{
				StatusInstance status = statuses[i];

				status.Tick(Time.deltaTime);

				if (status.IsExpired)
				{
					entity.Events.Publish(new EntityStatusRemovedEvent(statuses[i]));
					statuses[i].OnRemove();
					statuses.RemoveAt(i);
				}
			}
		}

		public void Apply(
			StatusDefinition definition,
			Entity source)
		{
			StatusInstance existing =
				statuses.Find(
					x => x.Definition == definition
				);

			if (existing != null)
			{
				existing.AddStack();
				return;
			}

			StatusInstance instance =
				definition.CreateRuntime();

			instance.Initialize(
				entity,
				source
			);

			statuses.Add(instance);

			entity.Events.Publish(new EntityStatusAppliedEvent(instance));
		}

		public bool Has(
			StatusDefinition definition)
		{
			return statuses.Exists(
				x => x.Definition == definition
			);
		}

		public StatusInstance Get(
			StatusDefinition definition)
		{
			return statuses.Find(
				x => x.Definition == definition
			);
		}

		public void Remove(
			StatusDefinition definition)
		{
			statuses.RemoveAll(
				x => x.Definition == definition
			);
		}
	}
}