using UnityEngine;

namespace UnboundArcana.Core.Rooms
{
	public class ConnectorTilemapOverride : MonoBehaviour
	{
		[SerializeField]
		private GameObject openRoot;

		[SerializeField]
		private GameObject closedRoot;

		public GameObject OpenRoot => openRoot;
		public GameObject ClosedRoot => closedRoot;

		private void Awake()
		{
			ApplyClosed();
		}

		public void ApplyOpen()
		{
			if (openRoot != null)
				openRoot.SetActive(true);

			if (closedRoot != null)
				closedRoot.SetActive(false);
		}

		public void ApplyClosed()
		{
			if (openRoot != null)
				openRoot.SetActive(false);

			if (closedRoot != null)
				closedRoot.SetActive(true);
		}

		public bool IsValid()
		{
			return openRoot != null &&
				closedRoot != null;
		}

#if UNITY_EDITOR
		public void Assign(
			GameObject open,
			GameObject closed)
		{
			openRoot = open;
			closedRoot = closed;

			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}