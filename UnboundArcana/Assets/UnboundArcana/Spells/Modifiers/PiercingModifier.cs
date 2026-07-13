using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Spells.Modifiers
{
	public class PiercingModifier : IRuntimeObjectModifier
	{
		private readonly int hits;

		public bool ControlsMovement => false;

		public PiercingModifier(int hits)
		{
			this.hits = hits;
		}

		public void Initialize(
			SpellRuntimeObject runtimeObject)
		{
			if (runtimeObject is ProjectileRuntimeObject projectile)
			{
				projectile.SetRemainingHits(hits);
			}
		}

		public void Update(float deltaTime)
		{
		}

		public void Destroy()
		{
		}

		public void OnHit(HitEvent hitEvent)
		{
		}
	}
}