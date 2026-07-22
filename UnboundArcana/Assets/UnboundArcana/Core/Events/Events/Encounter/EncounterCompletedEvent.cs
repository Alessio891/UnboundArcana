using UnboundArcana.Core.Rooms;

namespace UnboundArcana.Core.Events
{
	public class EncounterCompletedEvent
	{
		public EncounterInstance Encounter { get; }

		public EncounterCompletedEvent(
			EncounterInstance encounter)
		{
			Encounter = encounter;
		}
	}
}