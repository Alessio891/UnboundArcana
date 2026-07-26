using UnboundArcana.Core.Entities;
using UnityEngine;

public class PlayerSpawnedEvent {
	public Entity player;
	public PlayerSpawnedEvent(Entity player) { this.player = player; }
}

public class PlayerRemovedEvent {

}