using UnityEngine;
using UnboundArcana.Core.Entities;
using UnboundArcana.Sandbox;
using UnityEngine.InputSystem;

namespace UnboundArcana.Player
{
	[RequireComponent(typeof(PlayerInput))]
	[RequireComponent(typeof(CharacterMotor))]
	public class PlayerController : EntityController
	{
		private PlayerInput playerInput;
		private Camera mainCamera;

		protected override void Awake()
		{
			base.Awake();
			playerInput = GetComponent<PlayerInput>();
			mainCamera = Camera.main;
		}

		private void OnEnable()
		{
			playerInput.CastStarted += OnCastStarted;
			playerInput.CastEnded += OnCastEnded;
			playerInput.InteractStarted += OnInteract;
		}

		private void OnDisable()
		{
			playerInput.CastStarted -= OnCastStarted;
			playerInput.CastEnded -= OnCastEnded;
			playerInput.InteractStarted -= OnInteract;
		}

		private void Update()
		{
			if (!playerInput.InputEnabled)
			{
				Motor.SetMovementIntent(
					Vector2.zero
				);
				return;
			}

			Motor.SetMovementIntent(
				playerInput.Movement
			);
			Vector3 mousePosition =
				mainCamera.ScreenToWorldPoint(
					Mouse.current.position.ReadValue()
				);

			Vector3 direction =
				mousePosition - transform.position;

			direction.z = 0f;
			Facing.SetDirection(direction);
			SpellCaster.SetAimDirection(direction);
		}
		private void OnInteract()
		{
			Interaction.Interact();
		}
		private void OnCastStarted()
		{
			SpellCaster.BeginCast();
		}

		private void OnCastEnded()
		{
			SpellCaster.EndCast();
		}
	}
}
