using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Core.Events
{
	public class RuntimeObjectSpawnedEvent : SpellEvent
	{
		public SpellRuntimeObject RuntimeObject { get; }

		public RuntimeObjectSpawnedEvent(
			SpellRuntimeObject runtimeObject)
		{
			RuntimeObject = runtimeObject;
		}
	}
}