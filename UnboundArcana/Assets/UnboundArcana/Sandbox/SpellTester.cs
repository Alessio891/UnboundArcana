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
		public SpellDefinition spellDefinition;
		public SpellModuleDefinition testModule;

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

		private void Start()
		{
			spellConfiguration = new SpellConfiguration(spellDefinition);

			if (testModule != null)
			{
				spellConfiguration.AddModule(testModule);
			}
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
			Debug.Log($"Projectile hit: {hitEvent.Target.name}");
		}
	}
}