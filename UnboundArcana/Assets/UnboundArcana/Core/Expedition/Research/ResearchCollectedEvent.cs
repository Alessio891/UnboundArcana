using UnboundArcana.Core.Research;

public class ResearchCollectedEvent
{
	public ResearchDefinition Research { get; }

	public ResearchCollectedEvent(
		ResearchDefinition research)
	{
		Research = research;
	}
}