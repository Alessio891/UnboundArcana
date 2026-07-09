using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Objects
{
	public abstract class SpellRuntimeObject
	{
		protected SpellInstance spell;
		protected GameObject view;

		
		public bool IsAlive { get; private set; } = true;

		public virtual void Initialize(SpellInstance spell)
		{
			this.spell = spell;
		}

		public virtual void Tick(float deltaTime)
		{
		}

		public virtual void UpdateView(Transform transform)
		{
		}

		public virtual void Destroy()
		{
			IsAlive = false;

			if (view != null)
			{
				Object.Destroy(view);
			}
		}
		public void SetView(GameObject view)
		{
			this.view = view;
		}
	}
}