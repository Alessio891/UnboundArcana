using System.Collections.Generic;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime
{
	public class SpellInstance
	{
		public SpellBehavior behavior;
		public List<SpellModule> modules = new();

		private readonly List<SpellRuntimeObject> runtimeObjects = new();

		public SpellEventBus Events { get; } = new();

		public void Initialize()
		{
			behavior.Initialize(this);

			foreach (SpellModule module in modules)
			{
				module.Initialize(this);
			}
		}

		public void AddRuntimeObject(SpellRuntimeObject runtimeObject)
		{
			runtimeObject.Initialize(this);
			runtimeObjects.Add(runtimeObject);
		}

		public void Tick(float deltaTime)
		{
			for (int i = runtimeObjects.Count - 1; i >= 0; i--)
			{
				SpellRuntimeObject runtimeObject = runtimeObjects[i];

				runtimeObject.Tick(deltaTime);

				if (!runtimeObject.IsAlive)
				{
					runtimeObjects.RemoveAt(i);
				}
			}
		}

		public void Cast()
		{
			Events.Publish(new CastEvent(this));

			behavior.Cast();
		}
	}
}