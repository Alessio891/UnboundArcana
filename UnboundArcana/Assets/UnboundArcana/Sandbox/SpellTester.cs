using UnityEngine;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Events;
using UnityEngine.InputSystem;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Spells.Modules;

namespace UnboundArcana.Sandbox
{
	public class SpellTester : MonoBehaviour
	{
		public SpellRuntimeManager RuntimeManager;
		public SpellDefinition projectileSpell;

		public SpellModuleDefinition fireModule;
		public SpellModuleDefinition explosionModule;
		public SpellModuleDefinition sizeModule;

		private SpellConfiguration spellConfiguration;
		private SpellInstance activeSpell;

		private UnboundArcanaControls controls;
		private Camera mainCamera;
		private bool isCasting;
		private Vector2 moveInput;

		public float MoveSpeed = 10.0f;

		private void Awake()
		{
			controls = new UnboundArcanaControls();
			mainCamera = Camera.main;
		}
		private void SelectSpell(int index)
		{
			spellConfiguration = new SpellConfiguration(projectileSpell);

			switch (index)
			{
				case 1:
					spellConfiguration.AddModule(fireModule);
					break;

				case 2:
					spellConfiguration.AddModule(explosionModule);
					break;

				case 3:
					spellConfiguration.AddModule(fireModule);
					spellConfiguration.AddModule(explosionModule);
					spellConfiguration.AddModule(sizeModule);
					break;
			}
			Debug.Log($"Selected test spell: {index}");
		}

		private void Start()
		{
			SelectSpell(1);
		}

		private SpellInstance CreateSpellInstance()
		{
			return SpellFactory.Create(
				spellConfiguration,
				new SpellRuntimeContext(
					RuntimeManager,
					RuntimeManager.GameEvents),
				gameObject
			);
		}

		private void OnEnable()
		{
			controls.Gameplay.Enable();

			controls.Gameplay.Cast.performed += OnCast;
			controls.Gameplay.Cast.canceled += OnCastEnd;

			controls.Gameplay.Move.performed += ctx =>
			{
				moveInput = ctx.ReadValue<Vector2>();
			};

			controls.Gameplay.Move.canceled += ctx =>
			{
				moveInput = Vector2.zero;
			};
		}

		private void OnDisable()
		{
			controls.Gameplay.Cast.performed -= OnCast;
			controls.Gameplay.Cast.canceled -= OnCastEnd;

			controls.Gameplay.Disable();
		}

		private void Update()
		{
			if (Keyboard.current.digit1Key.wasPressedThisFrame)
			{
				SelectSpell(1);
			}

			if (Keyboard.current.digit2Key.wasPressedThisFrame)
			{
				SelectSpell(2);
			}

			if (Keyboard.current.digit3Key.wasPressedThisFrame)
			{
				SelectSpell(3);
			}

			transform.position +=
				new Vector3(moveInput.x, moveInput.y, 0) *
				MoveSpeed *
				Time.deltaTime;

			if (moveInput.x != 0)
			{
				GetComponentInChildren<SpriteRenderer>().flipX =
					moveInput.x < 0;
			}

			if (!isCasting || activeSpell == null)
			{
				return;
			}

			Vector3 mousePosition =
				mainCamera.ScreenToWorldPoint(
					Mouse.current.position.ReadValue()
				);

			Vector3 direction =
				mousePosition - transform.position;

			direction.z = 0f;

			activeSpell.UpdateCast(
				new CastContext(
					gameObject,
					transform.position,
					direction
				)
			);
		}

		private void OnCast(
			InputAction.CallbackContext context)
		{
			isCasting = true;

			Vector3 mousePosition =
				mainCamera.ScreenToWorldPoint(
					Mouse.current.position.ReadValue()
				);

			Vector3 direction =
				mousePosition - transform.position;

			direction.z = 0f;

			activeSpell = CreateSpellInstance();

			if (RuntimeManager)
			{
				RuntimeManager.Register(activeSpell);
			}

			activeSpell.Events.Subscribe<HitEvent>(OnHit);

			activeSpell.Cast(
				new CastContext(
					gameObject,
					transform.position,
					direction
				)
			);
		}

		private void OnCastEnd(
			InputAction.CallbackContext context)
		{
			isCasting = false;

			if (activeSpell != null)
			{
				activeSpell.End();
				activeSpell = null;
			}
		}

		private void OnHit(HitEvent hitEvent)
		{
		}
	}
}