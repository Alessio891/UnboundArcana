using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Core.Stats;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using System.Collections.Generic;

namespace UnboundArcana.Core.Entities
{
	public class SpellLoadout
	{
		public List<SpellConfiguration> Spellconfigurations = new();

		public int CurrentSpell = 0;

		public void AddSpell(SpellConfiguration config)
		{
			if (Spellconfigurations.Contains(config))
				return;

			Spellconfigurations.Add(config);
		}

		public void AddSpell(SpellDefinition definition)
		{
			SpellConfiguration newConfig =
				new SpellConfiguration(definition);

			Spellconfigurations.Add(newConfig);
		}

		public SpellConfiguration GetCurrentSpell()
		{
			if (CurrentSpell < 0 ||
				CurrentSpell >= Spellconfigurations.Count)
			{
				Debug.LogError(
					$"Requested spell {CurrentSpell} in loadout with {Spellconfigurations.Count} spells.");

				return null;
			}

			return Spellconfigurations[CurrentSpell];
		}
	}

	public class SpellCaster : MonoBehaviour
	{
		public SpellRuntimeManager RuntimeManager =>
			GameRuntimeManager.Instance.SpellRuntimeManager;

		public SpellDefinition SpellDefinition;

		private SpellInstance activeSpell;

		private SpellLoadout spellLoadout;

		public SpellLoadout SpellLoadout =>
			spellLoadout;

		private Vector3 aimDirection;

		[SerializeField]
		private float castCooldown = 0.25f;

		private float castTimer;

		private void Awake()
		{
		}

		public void InitializeLoadout(
			List<SpellDefinition> spells)
		{
			spellLoadout = new SpellLoadout();

			foreach (var definition in spells)
			{
				Debug.Log(
					$"Adding spell {definition.name} to entity {name} at start");

				spellLoadout.AddSpell(definition);
			}
		}

		private void Update()
		{
			castTimer -= Time.deltaTime;

			if (activeSpell == null)
			{
				return;
			}

			activeSpell.UpdateCast(
				new CastContext(
					gameObject,
					transform.position,
					aimDirection
				)
			);
		}

		public void SetAimDirection(
			Vector3 direction)
		{
			aimDirection = direction.normalized;
		}

		public void BeginCast()
		{
			if (castTimer > 0f)
			{
				return;
			}

			if (spellLoadout.GetCurrentSpell().behavior == null)
				return;

			castTimer = castCooldown;

			activeSpell = CreateSpellInstance();

			if (RuntimeManager)
			{
				RuntimeManager.Register(activeSpell);
			}

			activeSpell.Cast(
				new CastContext(
					gameObject,
					transform.position,
					aimDirection
				)
			);
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

		private SpellInstance CreateSpellInstance()
		{
			SpellInstance instance =
				SpellFactory.Create(
					spellLoadout.GetCurrentSpell(),
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
				return;

			foreach (RunModifier modifier in player.Modifiers)
			{
				if (!TryGetSpellStat(
					modifier.Stat,
					out string stat))
				{
					continue;
				}
				Debug.Log("Applying run modifier");
				instance.Stats.AddModifier(
					new StatModifier(
						stat,
						modifier.Value,
						ConvertOperation(
							modifier.Operation),
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