using UnityEngine;

public class ResearchExperimentStationEvent
{
	public GameObject Station {  get; private set; }
	public ResearchExperimentStationEvent(GameObject station) { 
		Station = station;
	}
}
