using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnboundArcana.Player
{
	public class PlayerInput : MonoBehaviour
	{
		private UnboundArcanaControls controls;
		[SerializeField] private bool inputEnabled = true;

		public bool InputEnabled => inputEnabled;
		public Vector2 Movement { get; private set; }

		public event Action CastStarted;
		public event Action CastEnded;
		public event Action InteractStarted;

		private void Awake()
		{
			controls = new UnboundArcanaControls();
		}
		private void Update()
		{
			if (inputEnabled != controls.Gameplay.enabled)
			{
				if (inputEnabled) { controls.Gameplay.Enable(); }
				else { controls.Gameplay.Disable(); }
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
		}

		private void OnDisable()
		{
			controls.Gameplay.Move.performed -= OnMove;
			controls.Gameplay.Move.canceled -= OnMoveCanceled;

			controls.Gameplay.Cast.performed -= OnCastStarted;
			controls.Gameplay.Cast.canceled -= OnCastEnded;
			controls.Gameplay.Interaction.performed -= OnInteractStarted;
			controls.Gameplay.Disable();
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
		}

		private void OnCastEnded(InputAction.CallbackContext context)
		{
			CastEnded?.Invoke();
		}
	}
}