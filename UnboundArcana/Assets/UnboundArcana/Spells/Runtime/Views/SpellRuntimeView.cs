using UnityEngine;

namespace UnboundArcana.Spells.Runtime.Views
{
	public abstract class SpellRuntimeView : MonoBehaviour
	{
		public virtual void DestroyView()
		{
			Destroy(gameObject);
		}
	}
}
