namespace UnboundArcana.Core.Events
{
	public class WaveStartedEvent : SpellEvent
	{
		public int Wave { get; }

		public WaveStartedEvent(int wave)
		{
			Wave = wave;
		}
	}
}