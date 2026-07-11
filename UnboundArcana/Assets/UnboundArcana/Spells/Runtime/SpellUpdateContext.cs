using UnityEngine;

public class SpellUpdateContext
{
	public Vector3 Direction { get; }

	public SpellUpdateContext(Vector3 direction)
	{
		Direction = direction;
	}
}