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
		Collider2D[] hits =
			Physics2D.OverlapCircleAll(
				projectile.Position,
				range
			);

		foreach (Collider2D hit in hits)
		{
			if (hit.gameObject == previousTarget)
			{
				continue;
			}
			if (hit.GetComponent<Entity>() == null) continue;
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

			return hit.GetComponent<Entity>();
		}

		return null;
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