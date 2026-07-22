using System.Collections.Generic;
using UnboundArcana.Core.Entities;
using UnboundArcana.Spells.Data;

namespace UnboundArcana.Core.Runtime
{
	public class PlayerState
	{
		public EntityDefinition Definition { get; }

		public List<SpellConfiguration> Spells { get; } = new();

		public PlayerState(
			EntityDefinition definition)
		{
			Definition = definition;
		}
	}
}