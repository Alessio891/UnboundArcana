using UnityEngine;

public class GameDatabase : MonoBehaviour
{
	[SerializeField] SpellDataCatalog spells;
	public SpellDataCatalog Spells => spells;

	static GameDatabase instance;
	public static GameDatabase Instance => instance;
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
	}
}
