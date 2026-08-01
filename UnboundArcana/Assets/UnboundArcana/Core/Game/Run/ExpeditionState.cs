namespace UnboundArcana.Core.Expedition
{
	public enum ExpeditionState
	{
		None,
		Preparing,
		EnteringRoom,
		RoomActive,
		Reward,
		ChoosingNextRoom,
		Completed,
		Failed
	}

	public enum ExpeditionOutcome
	{
		Completed,
		Failed
	}

	public sealed class ExpeditionResult
	{
		public ExpeditionOutcome Outcome { get; }
		public string Reason { get; }

		public ExpeditionResult(ExpeditionOutcome outcome, string reason)
		{
			Outcome = outcome;
			Reason = reason;
		}
	}
}
