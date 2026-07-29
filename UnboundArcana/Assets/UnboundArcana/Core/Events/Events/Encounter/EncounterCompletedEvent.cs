using UnboundArcana.Core.Rooms;

namespace UnboundArcana.Core.Events
{
	public class EncounterStartEvent {  public EncounterStartEvent() { } }
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