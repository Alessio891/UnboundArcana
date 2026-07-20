using UnboundArcana.Core.Entities;
using UnityEngine;

public class ResearchExperimentStationEvent
{
	public GameObject Station {  get; private set; }
	public Entity Entity { get; private set; }
	public ResearchExperimentStationEvent(GameObject station, Entity user) { 
		Station = station;
		Entity = user;
	}
}
