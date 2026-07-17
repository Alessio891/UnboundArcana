using UnityEngine;
using UnboundArcana.Core.Entities;

namespace UnboundArcana.Core.Entities.AI.Steering
{
	public abstract class SteeringStrategy
	{
		protected AIController Controller { get; }

		protected SteeringStrategy(
			AIController controller)
		{
			Controller = controller;
		}

		public abstract Vector2 CalculateDirection(
			Entity target);
	}
	public class DirectChaseSteering : SteeringStrategy
	{
		public DirectChaseSteering(
			AIController controller)
			: base(controller)
		{
		}


		public override Vector2 CalculateDirection(
			Entity target)
		{
			Vector2 direction =
				target.transform.position -
				Controller.transform.position;

			return direction.normalized;
		}
	}
	public class ErraticChaseSteering : SteeringStrategy
	{
		private readonly float amplitude;
		private readonly float frequency;

		private float time;


		public ErraticChaseSteering(
			AIController controller,
			float amplitude = 1f,
			float frequency = 3f)
			: base(controller)
		{
			this.amplitude = amplitude;
			this.frequency = frequency;
		}


		public override Vector2 CalculateDirection(
			Entity target)
		{
			time += Time.deltaTime;


			Vector2 toTarget =
				target.transform.position -
				Controller.transform.position;


			Vector2 direction =
				toTarget.normalized;


			Vector2 perpendicular =
				new Vector2(
					-direction.y,
					direction.x
				);


			float wave =
				Mathf.Sin(
					time * frequency
				)
				*
				amplitude;


			Vector2 result =
				direction +
				perpendicular * wave;


			return result.normalized;
		}
	}
}