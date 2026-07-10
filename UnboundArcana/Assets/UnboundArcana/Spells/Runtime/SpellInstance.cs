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

		public GameObject Owner { get; private set; }
		public SpellEventBus Events { get; } = new();
		public SpellRuntimeContext Runtime { get; }
		public ISpellSpawner Spawner { get; private set; }
		public SpellInstance(
			SpellRuntimeContext runtime,
			GameObject owner)
		{
			Runtime = runtime;
			Owner = owner;
		}

		public void Initialize()
		{
			behavior.Initialize(this);
			if (behavior is ISpellSpawner spawner)
			{
				Spawner = spawner;
			}
			foreach (SpellModule module in modules)
			{
				module.Initialize(this);
			}
		}

		public void RegisterRuntimeObject(SpellRuntimeObject runtimeObject)
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

		public void Cast(CastContext context)
		{
			Events.Publish(new CastEvent(this, context));

			behavior.Cast(context);
		}

		public void Destroy()
		{
			foreach (SpellModule module in modules)
			{
				module.Destroy();
			}
		}
	}
}