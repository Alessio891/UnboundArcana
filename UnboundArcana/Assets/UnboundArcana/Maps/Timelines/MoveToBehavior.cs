using UnboundArcana.Core.Entities;
using UnityEngine;
using UnityEngine.Playables;

public class MoveToBehaviour : PlayableBehaviour
{
	public ExposedReference<CharacterMotor> actor;
	public Vector2 targetPosition;

	private CharacterMotor motor;
	private bool started;
	PlayableGraph graph;
	public override void OnBehaviourPlay(Playable playable, FrameData info)
	{
		
		if (started)
			return;
		graph = playable.GetGraph();
		var resolver = playable.GetGraph().GetResolver();
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		motor = player.GetComponent<CharacterMotor>();// actor.Resolve(resolver);
		playable.GetGraph().GetRootPlayable(0).SetSpeed(0);
		if (motor == null)
			return;

		started = true;

		motor.MoveTo(targetPosition);
		motor.GetComponent<Entity>().Events.Subscribe<EntityMoveToCompleteEvent>(Complete);
	}

	private void Complete(EntityMoveToCompleteEvent evt)
	{
		//Debug.Log($"Playable valid? {playable.IsValid()}");
		motor.GetComponent<Entity>().Events.Unsubscribe<EntityMoveToCompleteEvent>(Complete);

		graph
			.GetRootPlayable(0)
			.SetSpeed(1);
	}

	public override void OnBehaviourPause(Playable playable, FrameData info)
	{
		started = false;
	}
}