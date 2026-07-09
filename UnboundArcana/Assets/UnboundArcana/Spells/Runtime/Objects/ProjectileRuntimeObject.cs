using UnityEngine;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ProjectileRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private Vector3 direction;
		private float speed = 10f;
		private float lifetime = 2f;
		private float elapsedTime;

		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);

			position = Vector3.zero;
			direction = Vector3.right;
		}

		public override void Tick(float deltaTime)
		{
			elapsedTime += deltaTime;

			position += direction * speed * deltaTime;

			if (view != null)
			{
				view.transform.position = position;
			}
			if (elapsedTime >= lifetime)
			{
				Destroy();
			}
		}


		public void Hit(GameObject target)
		{
			spell.Events.Publish(
				new HitEvent(position, target)
			);

			Destroy();
		}

	}
}