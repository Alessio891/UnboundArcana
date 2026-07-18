using System.Collections.Generic;
using UnboundArcana.Core.Entities.Statuses;
using UnboundArcana.Spells.Behaviors;
using UnboundArcana.Spells.Modules;
using UnityEngine;
[CreateAssetMenu(menuName = "Spells/Data/Catalog")]
public class SpellDataCatalog : ScriptableObject
{
	public List<SpellModuleDefinition> modules;
	public List<SpellBehaviorDefinition> behaviors;
	public List<StatusDefinition> status;
}
