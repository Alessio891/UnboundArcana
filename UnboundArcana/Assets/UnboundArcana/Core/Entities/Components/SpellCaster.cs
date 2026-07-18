using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using System;
using System.Collections.Generic;

namespace UnboundArcana.Core.Entities
{

	public class SpellLoadout {
		public List<SpellConfiguration> Spellconfigurations = new();

		public int CurrentSpell = 0;

		public void AddSpell(SpellConfiguration config) { 
			if (Spellconfigurations.Contains(config)) return;

			Spellconfigurations.Add(config); 
		}
		public void AddSpell(SpellDefinition definition) {
			SpellConfiguration newConfig = new SpellConfiguration(definition);
			Spellconfigurations.Add(newConfig);
		}

		public SpellConfiguration GetCurrentSpell() {
			if (CurrentSpell < 0 || CurrentSpell >= Spellconfigurations.Count) return null;

			return Spellconfigurations[CurrentSpell];
		}
	}

	public class SpellCaster : MonoBehaviour
	{
		public SpellRuntimeManager RuntimeManager;
		public SpellDefinition SpellDefinition;

		private SpellInstance activeSpell;
		
		private SpellLoadout spellLoadout;
		public SpellLoadout SpellLoadout => spellLoadout;

		private Vector3 aimDirection;

		[SerializeField]
		private float castCooldown = 0.25f;

		private float castTimer;

		private void Awake()
		{
		}

		public void  InitializeLoadout(List<SpellDefinition> spells) {
			spellLoadout = new SpellLoadout();

			foreach (var definition in spells) { 
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
			if (spellLoadout.GetCurrentSpell().behavior == null) return;

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
			return SpellFactory.Create(
				spellLoadout.GetCurrentSpell(),
				new SpellRuntimeContext(
					RuntimeManager,
					RuntimeManager.GameEvents),
				gameObject
			);
		}
	}
}