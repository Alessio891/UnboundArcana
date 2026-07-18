using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;
using UnityEngine;

public class GameReward
{
	public Sprite icon;
	public int cost;
}

public class SpellBehaviorReward : GameReward {
	public SpellBehaviorDefinition behavior;
}

public class SpellModuleReward : GameReward {
	public SpellModuleDefinition module;
}
