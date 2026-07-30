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

		private float castTimer;

		public GameObject Owner { get; private set; }
		public SpellEventBus Events { get; } = new();
		public SpellRuntimeContext Runtime { get; }
		public ISpellSpawner Spawner { get; private set; }
		public SpellStatCollection Stats { get; } = new();

		public bool HasBeenCast { get; private set; }
		public bool IsCasting { get; private set; }
		public bool IsFinished { get; private set; }
		public bool RequiresContinuousControl => behavior is IContinuousSpellBehavior;

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

				if (module is ISpellModifierProvider provider)
				{
					foreach (StatModifier modifier in provider.GetModifiers())
					{
						Stats.AddModifier(modifier);
					}
				}
			}
		}


		public void BeginCast(
			CastContext context)
		{
			if (HasBeenCast || IsCasting || IsFinished)
			{
				return;
			}

			float castTime =
				Stats.Get(
					StatKeys.Spell.CastTime
				);

			if (castTime <= 0f)
			{
				Cast(context);
				return;
			}

			IsCasting = true;
			castTimer = castTime;
		}


		public void TickCast(
			float deltaTime, CastContext context)
		{
			if (!IsCasting)
			{
				return;
			}

			castTimer -= deltaTime;

			if (castTimer <= 0f)
			{
				IsCasting = false;

				Cast(
					context
				);
			}
		}


		public void RegisterRuntimeObject(
			SpellRuntimeObject runtimeObject)
		{
			runtimeObject.Initialize(this);
			runtimeObjects.Add(runtimeObject);

			Events.Publish(
				new RuntimeObjectSpawnedEvent(runtimeObject)
			);
		}


		public void Tick(
			float deltaTime)
		{
			
			for (int i = runtimeObjects.Count - 1; i >= 0; i--)
			{
				SpellRuntimeObject runtimeObject =
					runtimeObjects[i];

				runtimeObject.Tick(deltaTime);

				if (!runtimeObject.IsAlive)
				{
					runtimeObjects.RemoveAt(i);
				}
			}

			if (HasBeenCast &&
				runtimeObjects.Count == 0 &&
				!IsFinished)
			{
				IsFinished = true;

				Events.Publish(
					new SpellFinishedEvent(this)
				);
			}
		}


		public void UpdateCast(
			CastContext context)
		{
			if (!HasBeenCast || IsFinished || !RequiresContinuousControl)
			{
				return;
			}

			behavior.UpdateCast(context);
		}


		public void Cast(
			CastContext context)
		{
			if (HasBeenCast || IsCasting || IsFinished)
			{
				return;
			}

			HasBeenCast = true;
			Runtime.RuntimeManager.Register(this);

			Events.Publish(
				new CastEvent(this, context)
			);

			behavior.Cast(context);
		}


		public void End()
		{
			if (IsFinished)
			{
				return;
			}

			if (IsCasting)
			{
				IsCasting = false;
				IsFinished = true;
				Destroy();
				return;
			}

			if (HasBeenCast && RequiresContinuousControl)
			{
				behavior.End();
			}
		}


		public void Destroy()
		{
			behavior.Destroy();

			foreach (SpellModule module in modules)
			{
				module.Destroy();
			}
		}
	}
}
