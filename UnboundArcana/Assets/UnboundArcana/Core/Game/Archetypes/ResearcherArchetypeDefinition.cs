using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(menuName = "Unbound Arcana/Researchers/Archetype")]
public class ResearcherArchetypeDefinition : ScriptableObject
{
	[SerializeField] private Sprite characterArt;
	[SerializeField] private string name;
	[SerializeField] private string description;

	[SerializeField] private List<string> innates;
	[SerializeField] private List<string> perks;

	public Sprite CharacterArt => characterArt;
	public string Name => name;
	public string Description => description;
	public List<string> Perks => perks;
	public List<string> Innates => innates;
}
