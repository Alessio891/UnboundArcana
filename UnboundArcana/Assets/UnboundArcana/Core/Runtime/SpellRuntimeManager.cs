using System.Collections.Generic;
using UnityEngine;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Core.Runtime
{
	public class SpellRuntimeManager : MonoBehaviour
	{
		private readonly List<SpellInstance> spells = new();

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