using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;
using UnityEngine;

namespace UnboundArcana.Spells.Data
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Spell Definition")]
	public class SpellDefinition : ScriptableObject
	{
		public SpellBehaviorDefinition behavior;
		public SpellModuleDefinition[] modules;

		[SerializeField]
		public float cooldown = 0.25f;
	}
}