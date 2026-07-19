using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Runtime;
using UnboundArcana.Spells.Services;
using UnityEngine;

[RequireComponent(typeof(SpellRuntimeManager))]
[RequireComponent(typeof(GameDatabase))]
[DefaultExecutionOrder(-1000)]
public class GameRuntimeManager : MonoBehaviour
{
	public GameEventBus Events { get; private set; }
	public DamageSystem Damage { get; private set; }
	public SpellModificationService SpellModification { get; private set; }
	[SerializeField]
	private DamageTextView damageTextPrefab;
	public DamageTextSystem DamageText { get; private set; }

	private static GameRuntimeManager instance;
	public static GameRuntimeManager Instance => instance;
	[SerializeField]
	private ModuleRewardTable RewardTable;
	public ModuleRewardService ModuleReward { get; private set; }

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
		SpellModification = new SpellModificationService(Events);
		Damage.Initialize(Events);
		DamageText = new DamageTextSystem(Events, damageTextPrefab);

		ModuleReward = new ModuleRewardService(RewardTable);
		DontDestroyOnLoad(gameObject);
	}
}
