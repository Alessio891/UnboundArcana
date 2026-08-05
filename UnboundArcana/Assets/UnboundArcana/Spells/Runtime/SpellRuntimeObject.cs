using System.Collections.Generic;
using UnboundArcana.Core.Visuals;
using UnboundArcana.Core.Events;
using UnboundArcana.Spells.Runtime.Views;
using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public abstract class SpellRuntimeObject
	{
		protected SpellInstance spell;
		protected SpellRuntimeView view;

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
			if (!IsAlive)
			{
				return;
			}

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
				view.DestroyView();
			}
		}

		public void SetView(SpellRuntimeView view)
		{
			this.view = view;
		}

		public void SetVisualAppearance(Sprite sprite, RuntimeAnimatorController animatorController, Color fallbackColor)
		{
			SetVisualStyle(fallbackColor, ProceduralPalette.SpellAccent(fallbackColor));
		}

		public void SetVisualStyle(Color color, Color accentColor)
		{
			if (view == null) { return; }

			view.ApplyVisualStyle(color, accentColor);
		}

		public void PublishHit(HitEvent hitEvent)
		{
			NotifyHit(hitEvent);
			spell.Events.Publish(hitEvent);
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
