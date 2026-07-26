using UnityEngine;
using UnboundArcana.Core.Entities;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Core.Camera;
using UnboundArcana.Core.Expedition;

namespace UnboundArcana.Core.Rooms
{
	public class TowerSceneController : MonoBehaviour
	{
		[SerializeField]
		private ExpeditionRuntimeController expedition;


		private void Start()
		{
			expedition.StartExpedition();
		}
	}
}