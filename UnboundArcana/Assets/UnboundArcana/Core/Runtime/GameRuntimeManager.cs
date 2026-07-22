using UnboundArcana.Core.Combat;
using UnboundArcana.Core.Events;
using UnboundArcana.Core.Rooms;
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
	public EntitySpawnService EntitySpawn { get; private set; }

	private static GameRuntimeManager instance;
	public static GameRuntimeManager Instance => instance;
	[SerializeField]
	private ModuleRewardTable RewardTable;
	public ModuleRewardService ModuleReward { get; private set; }
	private SpellRuntimeManager spellRuntimeManager;
	public SpellRuntimeManager SpellRuntimeManager => spellRuntimeManager;
	public RoomService Rooms { get; private set; }
	public PlayerSpawner PlayerSpawner { get; private set; }
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
		Rooms = new RoomService(new RoomGenerator(0.3f), Events);
		SpellModification = new SpellModificationService(Events);
		Damage.Initialize(Events);
		DamageText = new DamageTextSystem(Events, damageTextPrefab);
		spellRuntimeManager = GetComponent<SpellRuntimeManager>();
		ModuleReward = new ModuleRewardService(RewardTable);
		EntitySpawn = new EntitySpawnService();
		PlayerSpawner =	new PlayerSpawner();
		DontDestroyOnLoad(gameObject);
	}
}
