using UnityEngine;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Core.Events;

namespace UnboundArcana.Sandbox
{
	public class SpellTester : MonoBehaviour
	{
		public SpellRuntimeManager RuntimeManager;
		public SpellDefinition spellDefinition;

		private SpellInstance spell;
		private UnboundArcanaControls controls;

		private void Awake()
		{
			controls = new UnboundArcanaControls();
		}

		private void Start()
		{
			spell = SpellFactory.Create(spellDefinition, RuntimeManager.GameEvents, gameObject);
			if (RuntimeManager) RuntimeManager.Register(spell);
			spell.Events.Subscribe<HitEvent>(OnHit);
		}

		private void OnEnable()
		{
			controls.Gameplay.Enable();
			controls.Gameplay.Cast.performed += OnCast;
		}

		private void OnDisable()
		{
			controls.Gameplay.Cast.performed -= OnCast;
			controls.Gameplay.Disable();
		}

		private void OnCast(UnityEngine.InputSystem.InputAction.CallbackContext context)
		{
			spell.Cast();
		}
		private void OnHit(HitEvent hitEvent)
		{
			Debug.Log($"Projectile hit: {hitEvent.Target.name}");
		}
	}
}