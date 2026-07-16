using UnityEngine;

public class RunConfiguration
{
	private int researcherId = 0;
	private int towerInstability = 0;
	private int towerResearchMode = 0;

	public int ResearcherId => researcherId;
	public int TowerInstability => towerInstability;
	public int TowerResearchMode => towerResearchMode;

	public RunConfiguration(int researcherId, int towerInstability, int towerResearchMode)
	{
		this.researcherId = researcherId;
		this.towerInstability = towerInstability;
		this.towerResearchMode = towerResearchMode;
	}
	public RunConfiguration() { }

	public void SetResearcherId(int id) {
		researcherId = id; 
	}

	public void SetTowerInstability(int instab) { towerInstability = instab; }
	public void SetTowerResearchMode(int mode) { towerResearchMode = mode; }
}
