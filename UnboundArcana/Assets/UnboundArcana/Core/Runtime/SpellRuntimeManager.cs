using System.Collections.Generic;
using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Combat;

namespace UnboundArcana.Core.Runtime
{
	public class SpellRuntimeManager : MonoBehaviour
	{
		private readonly List<SpellInstance> spells = new();
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
			spells.Add(spell);
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;

			foreach (SpellInstance spell in spells)
			{
				spell.Tick(deltaTime);
			}
		}
	}
}