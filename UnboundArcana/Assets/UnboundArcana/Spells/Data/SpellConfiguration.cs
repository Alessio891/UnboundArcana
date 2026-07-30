using System.Collections.Generic;

using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;
using UnityEngine;

namespace UnboundArcana.Spells.Data
{
	public class SpellConfiguration
	{
		public SpellBehaviorDefinition behavior;
		public List<SpellModuleDefinition> modules = new();

		public float Cooldown = 0.25f;
		private float cooldownTimer;

		public SpellConfiguration(
			SpellDefinition definition)
		{
			behavior = definition.behavior;

			if (definition.modules != null)
			{
				modules.AddRange(definition.modules);
			}

			Cooldown = definition.cooldown;
		}

		public void TickCooldown(float deltaTime)
		{
			if (cooldownTimer > 0)
			{
				cooldownTimer -= deltaTime;
			}
		}

		public bool CanCast()
		{
			return cooldownTimer <= 0;
		}

		public void StartCooldown()
		{
			cooldownTimer = Cooldown;
			Debug.Log($"Starting CD of {Cooldown}");
		}

		public void SetBehavior(
			SpellBehaviorDefinition behavior)
		{
			this.behavior = behavior;
		}

		public void AddModule(
			SpellModuleDefinition module)
		{
			if (module == null)
			{
				return;
			}

			modules.Add(module);
		}

		public void RemoveModule(
			SpellModuleDefinition module)
		{
			if (module == null)
			{
				return;
			}

			modules.Remove(module);
		}

		public bool HasModule(
			SpellModuleDefinition module)
		{
			return modules.Contains(module);
		}
	}
}