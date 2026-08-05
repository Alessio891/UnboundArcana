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

		private float castElapsed;

		private const float CastTimeReference = 0.6f;
		private const float DamageReference = 10f;
		private const float DurationReference = 5f;
		private const float SpeedReference = 8f;
		private const float MinimumChargeEffect = 0.2f;

		public GameObject Owner { get; private set; }
		public SpellEventBus Events { get; } = new();
		public SpellRuntimeContext Runtime { get; }
		public ISpellSpawner Spawner { get; private set; }
		public SpellStatCollection Stats { get; } = new();

		public bool HasBeenCast { get; private set; }
		public bool IsCasting { get; private set; }
		public bool IsFinished { get; private set; }
		public float CastDuration { get; private set; }
		public float CastChargeProgress { get; private set; }
		public float ChargeEffectMultiplier => Mathf.Lerp(MinimumChargeEffect, 1f, CastChargeProgress);
		public float CastBurden => CalculateCastBurden();
		public float ChargeMovementMultiplier => RequiresContinuousControl || !IsCasting ? 1f : 1f - 0.58f * CastChargeProgress * CastBurden;
		public float ReleaseImpulse => RequiresContinuousControl ? 0f : Mathf.Pow(CastChargeProgress, 2f) * CastBurden * 2.2f;
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
			CastDuration = Mathf.Max(0f, castTime);
			castElapsed = 0f;
			CastChargeProgress = 0f;

			if (castTime <= 0f)
			{
				CastChargeProgress = 1f;
				Cast(context);
				return;
			}

			if (RequiresContinuousControl)
			{
				CastChargeProgress = 1f;
				Cast(context);
				return;
			}

			IsCasting = true;
		}


		public void TickCast(
			float deltaTime, CastContext context)
		{
			if (!IsCasting)
			{
				return;
			}

			castElapsed += Mathf.Max(0f, deltaTime);
			CastChargeProgress = CastDuration > 0f ? Mathf.Clamp01(castElapsed / CastDuration) : 1f;
		}

		public void Release(
			CastContext context)
		{
			if (!IsCasting || HasBeenCast || IsFinished)
			{
				return;
			}

			IsCasting = false;
			CastChargeProgress = CastDuration > 0f ? Mathf.Clamp01(castElapsed / CastDuration) : 1f;
			Cast(context);
		}

		public float GetChargedStat(string stat)
		{
			if (stat == StatKeys.Spell.CastTime || stat == StatKeys.Spell.Duration)
			{
				return Stats.Get(stat);
			}

			return Stats.Get(stat) * ChargeEffectMultiplier;
		}

		private float CalculateCastBurden()
		{
			float castTimeWeight = Mathf.Clamp01(Stats.Get(StatKeys.Spell.CastTime) / CastTimeReference);
			float damageWeight = Mathf.Clamp01(Stats.Get(StatKeys.Spell.Damage) / DamageReference);
			float sizeWeight = Mathf.Clamp01((Stats.Get(StatKeys.Spell.Size) - 0.3f) / 1.5f);
			float durationWeight = Mathf.Clamp01(Stats.Get(StatKeys.Spell.Duration) / DurationReference);
			float speedWeight = Mathf.Clamp01(Stats.Get(StatKeys.Spell.Speed) / SpeedReference);
			float burden = castTimeWeight * 0.55f + damageWeight * 0.2f + sizeWeight * 0.1f + durationWeight * 0.15f;
			return burden * Mathf.Lerp(1f, 0.88f, speedWeight);
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
				if (HasBeenCast && RequiresContinuousControl)
				{
					behavior.End();
					Destroy();
					return;
				}

				IsFinished = true;
				Destroy();
				return;
			}

			if (HasBeenCast && RequiresContinuousControl)
			{
				behavior.End();
				Destroy();
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
