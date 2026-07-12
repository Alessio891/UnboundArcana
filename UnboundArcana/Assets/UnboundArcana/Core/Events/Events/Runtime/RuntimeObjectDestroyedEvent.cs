using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Core.Events
{
	public class RuntimeObjectDestroyedEvent : SpellEvent
	{
		public SpellRuntimeObject RuntimeObject { get; }

		public RuntimeObjectDestroyedEvent(
			SpellRuntimeObject runtimeObject)
		{
			RuntimeObject = runtimeObject;
		}
	}
}