using UnboundArcana.Core.Stats;

namespace UnboundArcana.Core.Stats
{
	public interface IStatProvider
	{
		void ApplyStats(
			StatCollection stats
		);
	}
}