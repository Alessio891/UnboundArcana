using UnityEngine;


public class MapConstructionEvent
{
	bool constructed;
	public bool IsConstructing => constructed;
	public MapConstructionEvent(bool constructed) { this.constructed = constructed; }
}
