using System.Collections.Generic;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Runtime
{
	public interface ISpellModifierProvider
	{
		IEnumerable<StatModifier> GetModifiers();
	}
}