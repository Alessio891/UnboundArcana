using UnboundArcana.Core.Entities;
using UnityEngine;

namespace UnboundArcana.Core.Entities.AI.Attacks
{
	public abstract class AttackStrategy
	{
		protected AIController Controller { get; }

		protected AttackStrategy(
			AIController controller)
		{
			Controller = controller;
		}

		public abstract void Execute(Entity target);

		public virtual bool CanAttack(Entity target)
		{
			return true;
		}
	}

	public class SpellAttack : AttackStrategy
	{
		public SpellAttack(
			AIController controller)
			: base(controller)
		{
		}


		public override void Execute(
			Entity target)
		{
			Vector3 direction =
				target.transform.position -
				Controller.transform.position;


			Controller.FacingDirection.SetDirection(
				direction
			);

			Controller.Caster.SetAimDirection(
				direction
			);

			Controller.Caster.BeginCast();
		}
	}
	public class ContactDamageAttack
	: AttackStrategy
	{
		public ContactDamageAttack(
			AIController controller)
			: base(controller)
		{
		}


		public override void Execute(Entity target)
		{
		}
	}
}