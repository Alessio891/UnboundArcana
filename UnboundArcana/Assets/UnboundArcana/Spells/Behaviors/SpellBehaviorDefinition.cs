using UnityEngine;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Stats;

namespace UnboundArcana.Spells.Behaviors
{
	public abstract class SpellBehaviorDefinition : ScriptableObject, IStatProvider
	{
		[SerializeField] private Sprite icon;
		public Sprite Icon => icon;
		[SerializeField] protected float castTime = 0.0f;

		public abstract SpellBehavior CreateRuntime();

		public virtual void ApplyStats(
			StatCollection stats)
		{
		}
	}
}