using System;
using UnboundArcana.Core.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnboundArcana.Player
{
	public class PlayerInputStateChangedEvent
	{
		public bool isEnabled = false;
		public PlayerInputStateChangedEvent(bool e) { isEnabled = e; }
	}

	public class PlayerInput : MonoBehaviour
	{
		private UnboundArcanaControls controls;
		[SerializeField] private bool inputEnabled = true;

		public bool InputEnabled => inputEnabled;
		public Vector2 Movement { get; private set; }
		private Entity entity; 
		
		public event Action CastStarted;
		public event Action CastEnded;
		public event Action InteractStarted;
		public event Action<int> SpellSelected;

		bool isCasting = false;

		private void Awake()
		{
			controls = new UnboundArcanaControls();
			entity = GetComponent<Entity>();
		}
		public void SetInputEnabled(bool enabled) {
			bool stateChanged = inputEnabled != enabled;
			inputEnabled = enabled;
			if (controls != null) {
				if (enabled) { controls.Gameplay.Enable(); }
				else { controls.Gameplay.Disable(); }
			}
			if (!enabled) {
				Movement = Vector2.zero;
				if (isCasting) {
					isCasting = false;
					CastEnded?.Invoke();
				}
				if (stateChanged) { entity?.Events.Publish(new PlayerInputStateChangedEvent(InputEnabled)); }
			}
		}
		private void Update()
		{
			if (inputEnabled != controls.Gameplay.enabled)
			{
				if (inputEnabled) { controls.Gameplay.Enable(); }
				else { controls.Gameplay.Disable(); }
			}
			if (inputEnabled && isCasting) {
				CastStarted?.Invoke();
			}
		}
		private void OnEnable()
		{
			controls.Gameplay.Enable();

			controls.Gameplay.Move.performed += OnMove;
			controls.Gameplay.Move.canceled += OnMoveCanceled;

			controls.Gameplay.Cast.performed += OnCastStarted;
			controls.Gameplay.Cast.canceled += OnCastEnded;
			
			controls.Gameplay.Interaction.performed += OnInteractStarted;
			controls.Gameplay.SelectSpell1.performed += OnSelectSpell1;
			controls.Gameplay.SelectSpell2.performed += OnSelectSpell2;
			controls.Gameplay.SelectSpell3.performed += OnSelectSpell3;
			controls.Gameplay.SelectSpell4.performed += OnSelectSpell4;
		}

		private void OnDisable()
		{
			controls.Gameplay.Move.performed -= OnMove;
			controls.Gameplay.Move.canceled -= OnMoveCanceled;

			controls.Gameplay.Cast.performed -= OnCastStarted;
			controls.Gameplay.Cast.canceled -= OnCastEnded;
			
			controls.Gameplay.Interaction.performed -= OnInteractStarted;
			controls.Gameplay.SelectSpell1.performed -= OnSelectSpell1;
			controls.Gameplay.SelectSpell2.performed -= OnSelectSpell2;
			controls.Gameplay.SelectSpell3.performed -= OnSelectSpell3;
			controls.Gameplay.SelectSpell4.performed -= OnSelectSpell4;
			controls.Gameplay.Disable();
		}
		private void OnSelectSpell1(InputAction.CallbackContext context)
		{
			SpellSelected?.Invoke(0);
		}
		private void OnSelectSpell2(InputAction.CallbackContext context)
		{
			SpellSelected?.Invoke(1);
		}
		private void OnSelectSpell3(InputAction.CallbackContext context)
		{
			SpellSelected?.Invoke(2);
		}
		private void OnSelectSpell4(InputAction.CallbackContext context)
		{
			SpellSelected?.Invoke(3);
		}
		private void OnInteractStarted(
			InputAction.CallbackContext context)
		{
			InteractStarted?.Invoke();
		}
		private void OnMove(InputAction.CallbackContext context)
		{
			Movement = context.ReadValue<Vector2>();
		}

		private void OnMoveCanceled(InputAction.CallbackContext context)
		{
			Movement = Vector2.zero;
		}

		private void OnCastStarted(InputAction.CallbackContext context)
		{
			CastStarted?.Invoke();
			isCasting = true;
		}

		private void OnCastEnded(InputAction.CallbackContext context)
		{
			CastEnded?.Invoke();
			isCasting = false;
		}
	}
}
