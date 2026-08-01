namespace UnboundArcana.Core.Expedition
{
	public class ExpeditionRewardStartedEvent
	{
	}

	public sealed class ExpeditionEndedEvent
	{
		public ExpeditionResult Result { get; }

		public ExpeditionEndedEvent(ExpeditionResult result)
		{
			Result = result;
		}
	}

	public class ExpeditionRewardSelectedEvent
	{
		public GameReward Reward { get; }

		public ExpeditionRewardSelectedEvent(
			GameReward reward)
		{
			Reward = reward;
		}
	}
}
