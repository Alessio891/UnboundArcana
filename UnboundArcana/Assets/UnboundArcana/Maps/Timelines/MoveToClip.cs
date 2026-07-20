using UnboundArcana.Core.Entities;
using UnityEngine;
using UnityEngine.Playables;

public class MoveToClip : PlayableAsset
{
	public ExposedReference<CharacterMotor> actor;
	public Vector2 targetPosition;

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		var playable = ScriptPlayable<MoveToBehaviour>.Create(graph);

		var behaviour = playable.GetBehaviour();
		behaviour.actor = actor;
		behaviour.targetPosition = targetPosition;

		return playable;
	}
}