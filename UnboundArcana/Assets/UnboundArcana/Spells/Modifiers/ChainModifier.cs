using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime.Objects;
using UnboundArcana.Spells.Runtime;
using UnityEngine;
using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Entities;

public class ChainModifier : IRuntimeObjectModifier
{
	private ProjectileRuntimeObject projectile;
	private int remainingChains;
	private readonly float range;
	private readonly Collider2D[] targetBuffer = new Collider2D[32];

	public bool ControlsMovement => false;

	public ChainModifier(
		float range,
		int maxChains)
	{
		this.range = range;
		remainingChains = maxChains;
	}
	private Entity FindTarget(
			Entity previousTarget)
	{
		int hitCount = Physics2D.OverlapCircle(projectile.Position, range, ContactFilter2D.noFilter, targetBuffer);
		Entity closestTarget = null;
		float closestDistance = float.PositiveInfinity;

		for (int i = 0; i < hitCount; i++)
		{
			Collider2D hit = targetBuffer[i];
			Entity candidate = hit.GetComponent<Entity>();

			if (candidate == previousTarget)
			{
				continue;
			}
			if (candidate == null) continue;
			if (hit.GetComponent<IDamageable>() == null)
			{
				continue;
			}

			if (projectile.HitHistory.HasHit(hit.gameObject))
			{
				continue;
			}
			if (projectile.Spell.Owner == hit.gameObject) {
				continue;
			}

			float distance = (candidate.transform.position - projectile.Position).sqrMagnitude;
			if (distance >= closestDistance) { continue; }

			closestDistance = distance;
			closestTarget = candidate;
		}

		return closestTarget;
	}

	public void OnHit(HitEvent hitEvent)
	{
		if (remainingChains <= 0)
		{
			return;
		}

		Entity target = FindTarget(
			hitEvent.Target
		);

		if (target == null)
		{
			return;
		}

		remainingChains--;

		projectile.SetProjectileDirection(
			(target.transform.position - projectile.Position)
			.normalized
		);

		projectile.PreventDestroy();
	}
	public void Initialize(
			SpellRuntimeObject runtimeObject)
	{
		projectile =
			runtimeObject as ProjectileRuntimeObject;
	}

	public void Update(float deltaTime)
	{
	}

	public void Destroy()
	{
	}
}
