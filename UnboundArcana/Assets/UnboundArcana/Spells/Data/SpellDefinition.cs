using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;
using UnityEngine;

namespace UnboundArcana.Spells.Data
{
	[CreateAssetMenu(menuName = "Unbound Arcana/Spell Definition")]
	public class SpellDefinition : ScriptableObject
	{
		public SpellBehaviorDefinition behavior;
		public SpellModuleDefinition principle;
		public SpellModuleDefinition catalystA;
		public SpellModuleDefinition catalystB;
		public SpellModuleDefinition flux;

		[SerializeField]
		public float cooldown = 0.25f;
	}
}
