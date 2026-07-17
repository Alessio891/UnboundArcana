using UnityEngine;
using UnboundArcana.Spells.Data;
using UnboundArcana.Spells.Runtime;
using UnboundArcana.Core.Events;
using UnityEngine.InputSystem;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;

namespace UnboundArcana.Sandbox
{
	public class SpellTester : MonoBehaviour, IDamageable
	{
		public SpellRuntimeManager RuntimeManager;
		public SpellDefinition projectileSpell;

		public SpellModuleDefinition fireModule;
		public SpellModuleDefinition explosionModule;
		public SpellModuleDefinition sizeModule;


		private SpellConfiguration spellConfiguration;
		private SpellInstance activeSpell;

		private Camera mainCamera;
		private bool isCasting;
		private Vector2 moveInput;
		private CharacterMotor motor;
		public float MoveSpeed = 10.0f;
		public SpellConfiguration Configuration => spellConfiguration;
		[SerializeField] private float castCooldown = 0.25f;

		private float castTimer;
		private void Awake()
		{
			mainCamera = Camera.main;
			motor = GetComponent<CharacterMotor>();
		}

		private void Start()
		{
			spellConfiguration = new SpellConfiguration(projectileSpell);
			GetComponent<Entity>()
				.Stats
				.Set(EntityStatId.MaxHealth, 100);
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
			
		}

		private void OnDisable()
		{
			
		}
		private void Update()
		{
			
			castTimer -= Time.deltaTime;
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

		public void BeginCast()
		{
			if (castTimer > 0f)
			{
				return;
			}

			castTimer = castCooldown;

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

		public void EndCast()
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

		public void TakeDamage(DamageInfo damage)
		{
		}
	}
}