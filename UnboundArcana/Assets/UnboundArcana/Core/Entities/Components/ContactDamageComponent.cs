using UnityEngine;

namespace UnboundArcana.Core.Entities
{
	public class ContactDamageComponent : MonoBehaviour
	{
		[SerializeField]
		private float damage = 10f;


		private void OnCollisionStay2D(
			Collision2D collision)
		{
			Entity target =
				collision.gameObject
				.GetComponent<Entity>();

			if (target == null)
			{
				return;
			}

			// Damage event call here
		}
	}
}