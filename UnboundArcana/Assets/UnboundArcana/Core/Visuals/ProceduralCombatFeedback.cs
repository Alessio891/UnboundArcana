using UnboundArcana.Core.Camera;
using UnboundArcana.Core.Entities.Events;
using UnboundArcana.Core.Events;
using UnityEngine;

namespace UnboundArcana.Core.Visuals
{
	[DisallowMultipleComponent]
	public sealed class ProceduralCombatFeedback : MonoBehaviour
	{
		private GameEventBus events;
		private float hitStopTimer;
		private float previousTimeScale = 1f;
		private bool hitStopActive;

		public static ProceduralCombatFeedback Attach(GameObject host, GameEventBus events)
		{
			ProceduralCombatFeedback feedback = host.GetComponent<ProceduralCombatFeedback>();
			if (feedback == null)
			{
				feedback = host.AddComponent<ProceduralCombatFeedback>();
			}

			feedback.Initialize(events);
			return feedback;
		}

		private void Initialize(GameEventBus eventBus)
		{
			if (events == eventBus)
			{
				return;
			}

			Unsubscribe();
			events = eventBus;
			if (events == null)
			{
				return;
			}

			events.Subscribe<EntityDamagedEvent>(OnEntityDamaged);
			events.Subscribe<EntityDeathEvent>(OnEntityDeath);
		}

		private void OnDisable()
		{
			Unsubscribe();
			RestoreTimeScale();
		}

		private void OnDestroy()
		{
			Unsubscribe();
			RestoreTimeScale();
		}

		private void Unsubscribe()
		{
			if (events == null)
			{
				return;
			}

			events.Unsubscribe<EntityDamagedEvent>(OnEntityDamaged);
			events.Unsubscribe<EntityDeathEvent>(OnEntityDeath);
		}

		private void Update()
		{
			if (!hitStopActive)
			{
				return;
			}

			hitStopTimer -= Time.unscaledDeltaTime;
			if (hitStopTimer <= 0f)
			{
				RestoreTimeScale();
			}
		}

		private void OnEntityDamaged(EntityDamagedEvent evt)
		{
			if (evt.Entity == null || evt.Damage.Amount <= 0f)
			{
				return;
			}

			ProceduralVfx.SpawnImpact(evt.Entity.transform.position, ProceduralPalette.Damage, evt.Entity.CompareTag("Player") ? 0.8f : 1f);
			if (evt.Damage.Source != null && evt.Damage.Source.CompareTag("Player"))
			{
				RequestHitStop(0.035f, 0.05f);
				MainCameraManager.Instance?.Shake(0.035f, 0.06f);
			}
		}

		private void OnEntityDeath(EntityDeathEvent evt)
		{
			if (evt.Entity == null)
			{
				return;
			}

			Color color = evt.Entity.CompareTag("Player") ? ProceduralPalette.Overload : ProceduralPalette.EnemyAccent;
			ProceduralVfx.SpawnImpact(evt.Entity.transform.position, color, evt.Entity.CompareTag("Player") ? 1.5f : 1.2f);
			MainCameraManager.Instance?.Shake(evt.Entity.CompareTag("Player") ? 0.1f : 0.045f, evt.Entity.CompareTag("Player") ? 0.12f : 0.07f);
		}

		private void RequestHitStop(float duration, float timeScale)
		{
			if (Time.timeScale <= 0f)
			{
				return;
			}

			if (!hitStopActive)
			{
				previousTimeScale = Time.timeScale;
				hitStopActive = true;
			}

			hitStopTimer = Mathf.Max(hitStopTimer, duration);
			Time.timeScale = Mathf.Min(Time.timeScale, timeScale);
		}

		private void RestoreTimeScale()
		{
			if (!hitStopActive)
			{
				return;
			}

			Time.timeScale = previousTimeScale;
			hitStopActive = false;
			hitStopTimer = 0f;
		}
	}
}
