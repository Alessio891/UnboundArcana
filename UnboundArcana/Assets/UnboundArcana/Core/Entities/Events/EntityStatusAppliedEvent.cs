using UnboundArcana.Core.Entities.Statuses;
using UnityEngine;

public class EntityStatusAppliedEvent
{
	private StatusInstance statusInstance;
	public StatusInstance StatusInstance => statusInstance;

	public EntityStatusAppliedEvent(StatusInstance statusInstance) {  this.statusInstance = statusInstance; }

}
public class EntityStatusRemovedEvent
{
	private StatusInstance statusInstance;
	public StatusInstance StatusInstance => statusInstance;

	public EntityStatusRemovedEvent(StatusInstance statusInstance) { this.statusInstance = statusInstance; }
}