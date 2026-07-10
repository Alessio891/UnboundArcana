using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;

namespace UnboundArcana.Spells.Runtime
{
	public class SpellRuntimeContext
	{
		public ISpellRuntime RuntimeManager { get; }
		public GameEventBus GameEvents { get; }

		public SpellRuntimeContext(
			ISpellRuntime runtimeManager,
			GameEventBus gameEvents)
		{
			RuntimeManager = runtimeManager;
			GameEvents = gameEvents;
		}
	}
}