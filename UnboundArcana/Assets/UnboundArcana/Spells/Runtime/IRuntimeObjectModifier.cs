using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime.Objects;

namespace UnboundArcana.Spells.Runtime
{
	public interface IRuntimeObjectModifier
	{
		bool ControlsMovement { get; }
		void Initialize(
			SpellRuntimeObject runtimeObject
		);

		void Update(
			float deltaTime
		);

		void OnHit(
			HitEvent hitEvent
		);

		void Destroy();
	}
}