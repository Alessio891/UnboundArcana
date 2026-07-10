---

# Decision: Behavior Expansion Validation

Date:

2026-07-11


## Context

Projectile architecture was validated, but there was a risk of designing the entire system around projectile assumptions.


## Decision

Implement a non-projectile behavior before expanding the system further.


Implemented:

AuraBehavior


## Consequences

Positive:

- Behavior lifecycle is independent from movement and collision.
- Runtime object model works for persistent effects.
- Architecture is not projectile-specific.


---

# Decision: Runtime Context

Date:

2026-07-11


## Context

Spells require access to runtime services such as event buses and spell registration.


Direct references to scene managers would couple spells to Unity objects.


## Decision

Introduce SpellRuntimeContext.


Contains:

- GameEventBus
- Runtime registration capability


## Consequences

Positive:

- Spell runtime dependencies are explicit.
- SpellInstance does not depend directly on MonoBehaviours.
- Future services can be added without redesigning spell ownership.


---

# Decision: Spell Chaining Through Spell Composition

Date:

2026-07-11


## Context

Some effects require creating another spell rather than another runtime object.

Example:

Aura expiration creating a projectile nova.


## Decision

Allow modules to trigger new SpellInstances through SpellFactory.


The chained spell is a complete spell composition.


## Consequences

Positive:

- Behaviors remain independent.
- Modules do not need knowledge of other behaviors.
- Complex effects can be built from existing spell definitions.


Negative:

- Spell lifetime management will need future refinement.


---

# Decision: Generic Runtime Destruction Events

Date:

2026-07-11


## Context

Projectile-specific destruction events are insufficient for future behaviors.


Examples:

- Aura expiration
- Trap expiration
- Summon death


## Decision

Introduce RuntimeObjectDestroyedEvent.


Modules can react to runtime lifecycle without knowing the originating behavior.


## Consequences

Positive:

- Lifecycle modifiers become behavior-independent.


Negative:

- Additional lifecycle events may be needed later.