using UnboundArcana.Core.Interaction;
using UnityEngine;

namespace UnboundArcana.Core.Entities
{
	[RequireComponent(typeof(Entity))]
	[RequireComponent(typeof(CharacterMotor))]
	[RequireComponent(typeof(SpellCaster))]
	[RequireComponent(typeof(InteractionController))]
	[RequireComponent(typeof(TargetingComponent))]
	public abstract class EntityController : MonoBehaviour
	{
		protected Entity Entity { get; private set; }
		protected CharacterMotor Motor { get; private set; }
		protected SpellCaster SpellCaster { get; private set; }
		protected EntityFacing Facing { get; private set; }
		protected InteractionController Interaction { get; private set; }
		protected TargetingComponent Targeting { get; private set; }

		protected virtual void Awake()
		{
			Entity = GetComponent<Entity>();
			Motor = GetComponent<CharacterMotor>();
			SpellCaster = GetComponent<SpellCaster>();
			Facing = GetComponent<EntityFacing>();
			Interaction = GetComponent<InteractionController>();
			Targeting = GetComponent<TargetingComponent>();
		}
	}
}