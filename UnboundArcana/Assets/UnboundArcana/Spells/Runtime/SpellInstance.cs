using System.Collections.Generic;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Stats;
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
		public SpellStatCollection Stats { get; } = new();

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

			//Stats.AddBase(StatId.Size, 1, this);
			//Stats.AddBase(StatId.Damage, 1);

			foreach (SpellModule module in modules)
			{
				module.Initialize(this);

				if (module is ISpellModifierProvider provider)
				{
					foreach (StatModifier modifier in provider.GetModifiers())
					{
						Stats.AddModifier(modifier);
					}
				}
			}
		}

		public void RegisterRuntimeObject(SpellRuntimeObject runtimeObject)
		{
			runtimeObject.Initialize(this);
			runtimeObjects.Add(runtimeObject);

			Events.Publish(
				new RuntimeObjectSpawnedEvent(runtimeObject)
			);
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

		public void UpdateCast(CastContext context)
		{
			behavior.UpdateCast(context);
		}

		public void Cast(CastContext context)
		{
			Events.Publish(new CastEvent(this, context));

			behavior.Cast(context);
		}

		public void End()
		{
			behavior.End();
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