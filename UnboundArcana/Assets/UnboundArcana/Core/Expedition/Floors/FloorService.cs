namespace UnboundArcana.Core.Expedition
{
	public class FloorService
	{
		private readonly FloorGenerator generator;


		public FloorService()
		{
			generator =
				new FloorGenerator();
		}


		public FloorInstance GenerateFloor(
			FloorDefinition definition)
		{
			return generator.Generate(
				definition);
		}
	}
}