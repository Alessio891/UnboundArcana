using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Spells.Runtime.Objects;
using UnityEngine;

namespace UnboundArcana.Spells.Modules.Pulse
{
	public class PulseModifier : IRuntimeObjectModifier
	{
		private readonly float interval;

		private ExplosionRuntimeObject explosion;
		private float timer;

		public bool ControlsMovement => false;

		public PulseModifier(
			float interval)
		{
			this.interval = interval;
		}

		public void Initialize(
			SpellRuntimeObject runtimeObject)
		{
			explosion =
				runtimeObject as ExplosionRuntimeObject;
		}

		public void Update(float deltaTime)
		{
			if (explosion == null)
			{
				return;
			}

			timer += deltaTime;
			Debug.Log($"Pulse timer {timer}");
			if (timer < interval)
			{
				return;
			}

			timer -= interval;
			Debug.Log("Dealing expl damage");
			explosion.DealDamage();
		}

		public void OnHit(HitEvent hitEvent)
		{
		}

		public void Destroy()
		{
		}
	}
}