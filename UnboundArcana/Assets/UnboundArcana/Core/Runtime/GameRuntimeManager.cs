using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class GameRuntimeManager : MonoBehaviour
{
	public GameEventBus Events { get; private set; }
	public DamageSystem Damage { get; private set; }

	private static GameRuntimeManager instance;
	public static GameRuntimeManager Instance => instance;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		Events = new GameEventBus();
		Damage = new DamageSystem();
		Damage.Initialize(Events);
		DontDestroyOnLoad(gameObject);
	}
}
