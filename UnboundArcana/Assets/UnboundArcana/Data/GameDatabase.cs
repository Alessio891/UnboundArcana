using UnityEngine;

public class GameDatabase : MonoBehaviour
{
	[SerializeField] SpellDataCatalog spells;
	public SpellDataCatalog Spells => spells;
}
