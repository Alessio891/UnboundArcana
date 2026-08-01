using UnboundArcana.Core.Entities;

public class RunConfiguration
{
	public EntityDefinition PlayerDefinition { get; }

	private RunConfiguration(EntityDefinition playerDefinition)
	{
		PlayerDefinition = playerDefinition;
	}

	public static RunConfiguration CreateDefault(EntityDefinition playerDefinition)
	{
		return new RunConfiguration(playerDefinition);
	}
}
