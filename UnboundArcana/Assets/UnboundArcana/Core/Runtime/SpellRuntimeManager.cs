using System.Collections.Generic;
using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Combat;

namespace UnboundArcana.Core.Runtime
{
	public class SpellRuntimeManager : MonoBehaviour, ISpellRuntime
	{
		private readonly List<SpellInstance> spells = new();
		private readonly List<SpellInstance> pendingSpells = new();
		private GameRuntimeManager gameRuntimeManager;
		public GameEventBus GameEvents => gameRuntimeManager.Events;
		
		private DamageSystem damageSystem => gameRuntimeManager.Damage;
		private void Awake()
		{
			gameRuntimeManager = GetComponent<GameRuntimeManager>();
		}
		public void Register(SpellInstance spell)
		{
			spell.Events.Subscribe<SpellFinishedEvent>(
				OnSpellFinished
			);

			pendingSpells.Add(spell);
		}
		private void OnSpellFinished(
			SpellFinishedEvent eventData)
		{
			eventData.Spell.Events.Unsubscribe<SpellFinishedEvent>(
				OnSpellFinished
			);

			spells.Remove(eventData.Spell);

			eventData.Spell.Destroy();
		}
		private void Update()
		{
			if (pendingSpells.Count > 0)
			{
				spells.AddRange(pendingSpells);
				pendingSpells.Clear();
			}

			float deltaTime = Time.deltaTime;

			for (int i = spells.Count - 1; i >= 0; i--)
			{
				spells[i].Tick(deltaTime);
			}
		}
	}
}
