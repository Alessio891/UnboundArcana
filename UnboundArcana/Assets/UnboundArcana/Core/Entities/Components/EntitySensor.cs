using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnboundArcana.Core.Entities
{
	public class EntitySensor : MonoBehaviour
	{
		public event Action<Entity> EntityDetected;
		public event Action<Entity> EntityLost;

		private readonly HashSet<Entity> detectedEntities = new();

		public IReadOnlyCollection<Entity> DetectedEntities => detectedEntities;

		public bool IsDetected(Entity entity)
		{
			return entity != null && detectedEntities.Contains(entity);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.TryGetComponent<Entity>(out var entity))
				return;

			if (detectedEntities.Add(entity))
			{
				EntityDetected?.Invoke(entity);
			}
		}


		private void OnTriggerExit2D(Collider2D other)
		{
			if (!other.TryGetComponent<Entity>(out var entity))
				return;

			if (detectedEntities.Remove(entity))
			{
				EntityLost?.Invoke(entity);
			}
		}
	}
}
