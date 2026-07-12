using UnboundArcana.Core.Events;

namespace UnboundArcana.Core.Events
{
	public class EncounterCompletedEvent : SpellEvent
	{
		public int Wave { get; }

		public EncounterCompletedEvent(int wave)
		{
			Wave = wave;
		}
	}
}