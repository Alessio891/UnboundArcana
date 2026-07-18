using UnityEngine;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;

namespace UnboundArcana.Core.Entities
{
	public class SpellCaster : MonoBehaviour
	{
		public SpellRuntimeManager RuntimeManager;
		public SpellDefinition SpellDefinition;

		private SpellConfiguration configuration;
		private SpellInstance activeSpell;
		public SpellConfiguration SpellConfiguration => configuration;

		private Vector3 aimDirection;

		[SerializeField]
		private float castCooldown = 0.25f;

		private float castTimer;

		private void Awake()
		{
			configuration =
				new SpellConfiguration(
					SpellDefinition
				);
		}

		private void Update()
		{
			castTimer -= Time.deltaTime;

			if (activeSpell == null)
			{
				return;
			}

			activeSpell.UpdateCast(
				new CastContext(
					gameObject,
					transform.position,
					aimDirection
				)
			);
		}

		public void SetAimDirection(
			Vector3 direction)
		{
			aimDirection = direction.normalized;
		}

		public void BeginCast()
		{
			if (castTimer > 0f)
			{
				return;
			}

			castTimer = castCooldown;

			activeSpell = CreateSpellInstance();

			if (RuntimeManager)
			{
				RuntimeManager.Register(activeSpell);
			}

			activeSpell.Cast(
				new CastContext(
					gameObject,
					transform.position,
					aimDirection
				)
			);
		}

		public void EndCast()
		{
			if (activeSpell == null)
			{
				return;
			}

			activeSpell.End();
			activeSpell = null;
		}

		private SpellInstance CreateSpellInstance()
		{
			return SpellFactory.Create(
				configuration,
				new SpellRuntimeContext(
					RuntimeManager,
					RuntimeManager.GameEvents),
				gameObject
			);
		}
	}
}