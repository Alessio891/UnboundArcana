using UnityEngine;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Core.Events;
using UnityEngine.InputSystem;

namespace UnboundArcana.Sandbox
{
	public class SpellTester : MonoBehaviour
	{
		public SpellRuntimeManager RuntimeManager;
		public SpellDefinition spellDefinition;

		private SpellInstance spell;
		private UnboundArcanaControls controls;
		private Camera mainCamera;
		private void Awake()
		{
			controls = new UnboundArcanaControls();
			mainCamera = Camera.main;
		}

		private void Start()
		{
			spell = SpellFactory.Create(spellDefinition, new SpellRuntimeContext(RuntimeManager,RuntimeManager.GameEvents), gameObject);
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
			Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
				Mouse.current.position.ReadValue()
			);

			Vector3 direction = mousePosition - transform.position;
			direction.z = 0f;
			
			spell.Cast(
				new CastContext(
					gameObject,
					transform.position,
					direction
				)
			);
		}
		private void OnHit(HitEvent hitEvent)
		{
			Debug.Log($"Projectile hit: {hitEvent.Target.name}");
		}
	}
}