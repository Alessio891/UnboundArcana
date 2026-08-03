using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using System;
using System.Collections.Generic;

namespace UnboundArcana.Core.Entities
{
	public class SpellSlot
	{
		public SpellConfiguration Configuration { get; }

		private float cooldownTimer;
		public float CooldownTimer => cooldownTimer;

		public bool CanCast =>
			cooldownTimer <= 0f;

		public SpellSlot(
			SpellConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void StartCooldown()
		{
			cooldownTimer = Configuration.Cooldown;
		}

		public void Tick(
			float deltaTime)
		{
			if (cooldownTimer <= 0f)
			{
				return;
			}

			cooldownTimer = Mathf.Max(0f, cooldownTimer - deltaTime);
		}
	}

	public class SpellLoadout
	{
		private readonly List<SpellSlot> slots = new();

		public int CurrentSpell { get; private set; }
		public event Action<int> CurrentSpellChanged;
		public event Action SlotsChanged;

		public IReadOnlyList<SpellSlot> Slots =>
			slots;

		public void AddSpell(
			SpellConfiguration configuration)
		{
			if (configuration == null)
			{
				return;
			}

			slots.Add(
				new SpellSlot(configuration)
			);

			SlotsChanged?.Invoke();
		}

		public void AddSpell(
			SpellDefinition definition)
		{
			AddSpell(
				new SpellConfiguration(definition)
			);
		}

		public SpellSlot GetCurrentSpell()
		{
			if (CurrentSpell < 0 ||
				CurrentSpell >= slots.Count)
			{
				Debug.LogError(
					$"Requested spell {CurrentSpell} in loadout with {slots.Count} spells.");

				return null;
			}

			return slots[CurrentSpell];
		}

		public bool SelectSpell(int index)
		{
			if (index < 0 || index >= slots.Count || index == CurrentSpell)
			{
				return false;
			}

			CurrentSpell = index;
			CurrentSpellChanged?.Invoke(CurrentSpell);
			return true;
		}

		public void Tick(
			float deltaTime)
		{
			foreach (SpellSlot slot in slots)
			{
				slot.Tick(deltaTime);
			}
		}
	}


	public class SpellCaster : MonoBehaviour
	{
		public SpellRuntimeManager RuntimeManager =>
			GameRuntimeManager.Instance.SpellRuntimeManager;

		private SpellInstance activeSpell;

		private SpellLoadout spellLoadout;

		public SpellLoadout SpellLoadout =>
			spellLoadout;

		private Vector3 aimDirection;
		private bool ignoreNextCooldown;

		public void ArmNextCooldownBypass()
		{
			ignoreNextCooldown = true;
		}

		public void ClearCooldownBypass()
		{
			ignoreNextCooldown = false;
		}

		public void InitializeLoadout(
			List<SpellDefinition> spells)
		{
			spellLoadout = new SpellLoadout();

			foreach (var definition in spells)
			{
				spellLoadout.AddSpell(definition);
			}
		}

		private void Update()
		{
			if (spellLoadout == null)
			{
				return;
			}

			spellLoadout.Tick(Time.deltaTime);

			if (activeSpell == null)
			{
				return;
			}

			var ctx = new CastContext(
				gameObject,
				transform.position,
				aimDirection
			);

			if (activeSpell.IsCasting)
			{
				activeSpell.TickCast(Time.deltaTime, ctx);
			}

			if (activeSpell.HasBeenCast && activeSpell.RequiresContinuousControl)
			{
				activeSpell.UpdateCast(ctx);
			}

			if (!activeSpell.IsCasting && !activeSpell.RequiresContinuousControl)
			{
				activeSpell = null;
			}
		}

		public void SetAimDirection(
			Vector3 direction)
		{
			aimDirection = direction.normalized;
		}

		public void BeginCast()
		{
			if (spellLoadout == null || activeSpell != null)
			{
				return;
			}

			SpellSlot slot = spellLoadout.GetCurrentSpell();

			if (slot == null || !slot.CanCast)
			{
				return;
			}

			SpellConfiguration configuration = slot.Configuration;

			if (configuration.behavior == null)
			{
				return;
			}

			SpellInstance spell = CreateSpellInstance(configuration);

			if (spell == null)
			{
				return;
			}

			activeSpell = spell;

			if (ignoreNextCooldown)
				ignoreNextCooldown = false;
			else
				slot.StartCooldown();

			activeSpell.BeginCast(
				new CastContext(
					gameObject,
					transform.position,
					aimDirection
				)
			);

			if (!activeSpell.IsCasting && !activeSpell.RequiresContinuousControl)
			{
				activeSpell = null;
			}
		}

		public void EndCast()
		{
			if (activeSpell == null)
			{
				return;
			}

			activeSpell.End();
			activeSpell = null;
		}

		public void CancelActiveSpell()
		{
			EndCast();
		}

		private SpellInstance CreateSpellInstance(
			SpellConfiguration configuration)
		{
			SpellInstance instance =
				SpellFactory.Create(
					configuration,
					new SpellRuntimeContext(
						RuntimeManager,
						RuntimeManager.GameEvents),
					gameObject
				);

			ApplyRunModifiers(instance);

			return instance;
		}

		private void ApplyRunModifiers(
			SpellInstance instance)
		{
			PlayerState player =
				GameSession.Instance?.Player;

			if (player == null)
			{
				return;
			}

			foreach (RunModifier modifier in player.Modifiers)
			{
				if (!TryGetSpellStat(
					modifier.Stat,
					out string stat))
				{
					continue;
				}

				instance.Stats.AddModifier(
					new StatModifier(
						stat,
						modifier.Value,
						ConvertOperation(modifier.Operation),
						modifier
					)
				);
			}
		}

		private bool TryGetSpellStat(
			RunModifierStat modifierStat,
			out string stat)
		{
			switch (modifierStat)
			{
				case RunModifierStat.SpellDamage:
					stat = StatKeys.Spell.Damage;
					return true;

				case RunModifierStat.SpellSpeed:
					stat = StatKeys.Spell.Speed;
					return true;

				case RunModifierStat.SpellSize:
					stat = StatKeys.Spell.Size;
					return true;

				case RunModifierStat.SpellDuration:
					stat = StatKeys.Spell.Duration;
					return true;

				default:
					stat = default;
					return false;
			}
		}

		private ModifierOperation ConvertOperation(
			RunModifierOperation operation)
		{
			switch (operation)
			{
				case RunModifierOperation.Flat:
					return ModifierOperation.Flat;

				case RunModifierOperation.Percent:
					return ModifierOperation.Percent;

				default:
					return ModifierOperation.Flat;
			}
		}
	}
}
