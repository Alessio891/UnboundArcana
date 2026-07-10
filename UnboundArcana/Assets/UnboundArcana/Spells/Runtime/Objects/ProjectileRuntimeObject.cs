using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Combat;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public class ProjectileRuntimeObject : SpellRuntimeObject
	{
		private Vector3 position;
		private Vector3 direction;
		private float speed = 10f;
		private float lifetime = 2f;
		private float elapsedTime;
		private GameObject owner;


		public override void Initialize(SpellInstance spell)
		{
			base.Initialize(spell);
		}
		public void SetInitialState(
			Vector3 position,
			Vector3 direction,
			GameObject owner)
		{
			this.position = position;
			this.direction = direction;
			this.owner = owner;
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
			if (target != owner && target.GetComponent<IDamageable>() != null)
			{
				spell.Events.Publish(
					new HitEvent(position, target, owner)
				);

				Destroy();
			}
		}

	}
}