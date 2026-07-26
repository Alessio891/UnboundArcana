namespace UnboundArcana.Core.Expedition
{
	public class ExpeditionRewardStartedEvent
	{
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