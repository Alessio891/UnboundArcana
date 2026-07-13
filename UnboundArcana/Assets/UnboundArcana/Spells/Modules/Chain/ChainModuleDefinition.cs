using UnboundArcana.Spells.Modules.Chain;
using UnboundArcana.Spells.Modules;
using UnboundArcana.Spells.Runtime;
using UnityEngine;
[CreateAssetMenu(menuName = "Spells/Modules/Chain")]
public class ChainModuleDefinition : SpellModuleDefinition
{
	public float range = 5f;
	public int maxChains = 3;

	public override SpellModule CreateRuntime()
	{
		return new ChainModule(this);
	}
}