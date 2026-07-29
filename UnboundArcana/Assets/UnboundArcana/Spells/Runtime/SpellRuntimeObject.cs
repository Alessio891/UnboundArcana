using System.Collections.Generic;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime.Views;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public abstract class SpellRuntimeObject
	{
		protected SpellInstance spell;
		protected GameObject view;

		protected readonly List<IRuntimeObjectModifier> modifiers = new();

		public bool IsAlive { get; private set; } = true;
		public SpellInstance Spell { get { return spell; } }

		public virtual void Initialize(SpellInstance spell)
		{
			this.spell = spell;
		}

		public void AddModifier(
			IRuntimeObjectModifier modifier)
		{
			if (modifier == null)
			{
				return;
			}

			modifiers.Add(modifier);

			modifier.Initialize(this);
		}

		public virtual void Tick(float deltaTime)
		{
			foreach (IRuntimeObjectModifier modifier in modifiers)
			{
				modifier.Update(deltaTime);
			}
		}

		public virtual void UpdateView(Transform transform)
		{
		}

		public virtual void Destroy()
		{
			IsAlive = false;

			foreach (IRuntimeObjectModifier modifier in modifiers)
			{
				modifier.Destroy();
			}

			spell.Events.Publish(
				new RuntimeObjectDestroyedEvent(this)
			);

			if (view != null)
			{
				view.GetComponent<ProjectileView>().DestroyView();
				//Object.Destroy(view);
			}
		}

		public void SetView(GameObject view)
		{
			this.view = view;
		}

		public virtual void OnDestroyed()
		{
		}
		public void NotifyHit(
			HitEvent hitEvent)
		{
			foreach (IRuntimeObjectModifier modifier in modifiers)
			{
				modifier.OnHit(hitEvent);
			}
		}
	}
}