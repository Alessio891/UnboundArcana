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
		public GameEventBus GameEvents { get; private set; }
		
		private DamageSystem damageSystem;

		private void Awake()
		{
			GameEvents = new GameEventBus();
			damageSystem = new DamageSystem();
			damageSystem.Initialize(GameEvents);
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