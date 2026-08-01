using UnityEngine;

namespace UnboundArcana.Core.Runtime
{
	public class GameSession : MonoBehaviour
	{
		public static GameSession Instance { get; private set; }

		public PlayerState Player { get; private set; }
		public RunConfiguration CurrentRun { get; private set; }

		private void Awake()
		{
			if (Instance != null &&
				Instance != this)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;

			DontDestroyOnLoad(gameObject);
		}

		public void CreatePlayer(
			Entities.EntityDefinition definition)
		{
			Player =
				new PlayerState(definition);
		}

		public void BeginNewRun(RunConfiguration configuration)
		{
			CurrentRun = configuration;
			Player = new PlayerState(configuration.PlayerDefinition);
		}

		public void ClearRun()
		{
			CurrentRun = null;
			Player = null;
		}
	}
}
