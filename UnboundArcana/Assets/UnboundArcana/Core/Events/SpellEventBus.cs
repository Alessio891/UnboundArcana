using System;
using System.Collections.Generic;

namespace UnboundArcana.Core.Events
{
	public class SpellEventBus
	{
		private readonly Dictionary<Type, List<Delegate>> listeners = new();

		public void Subscribe<T>(Action<T> listener) where T : SpellEvent
		{
			Type type = typeof(T);

			if (!listeners.ContainsKey(type))
			{
				listeners[type] = new List<Delegate>();
			}

			listeners[type].Add(listener);
		}

		public void Unsubscribe<T>(Action<T> listener) where T : SpellEvent
		{
			Type type = typeof(T);

			if (!listeners.ContainsKey(type))
			{
				return;
			}

			listeners[type].Remove(listener);
		}

		public void Publish<T>(T eventData) where T : SpellEvent
		{
			Type type = typeof(T);

			if (!listeners.ContainsKey(type))
			{
				return;
			}
			var snapshot = listeners[type].ToArray();

			foreach (Delegate listener in snapshot)
			{
				((Action<T>)listener).Invoke(eventData);
			}
		}
	}
}