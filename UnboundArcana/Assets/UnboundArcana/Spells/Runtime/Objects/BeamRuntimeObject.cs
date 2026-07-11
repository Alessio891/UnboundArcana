using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class BeamRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private Vector3 direction;

		public Vector3 Position => position;
		public Vector3 Direction => direction;

		public void SetInitialState(
			Vector3 position,
			Vector3 direction
		)
		{
			this.position = position;
			this.direction = direction.normalized;
		}
		public void SetPosition(Vector3 position) {
			this.position = position;
			if (view != null)
			{
				UpdateView(view.transform);
			}
		}
		public void SetDirection(Vector3 direction)
		{
			this.direction = direction.normalized;

			if (view != null)
			{
				UpdateView(view.transform);
			}
		}
		public override void UpdateView(Transform transform)
		{
			transform.position = position;

			transform.rotation = Quaternion.Euler(
				0f,
				0f,
				Mathf.Atan2(
					direction.y,
					direction.x
				) * Mathf.Rad2Deg
			);
		}
	}
}