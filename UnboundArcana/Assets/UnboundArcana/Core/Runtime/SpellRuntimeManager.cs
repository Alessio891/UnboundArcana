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
			pendingSpells.Add(spell);
		}

		private void Update()
		{
			if (pendingSpells.Count > 0)
			{
				spells.AddRange(pendingSpells);
				pendingSpells.Clear();
			}

			float deltaTime = Time.deltaTime;

			for (int i = 0; i < spells.Count; i++)
			{
				spells[i].Tick(deltaTime);
			}
		}
	}
}