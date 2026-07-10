# Decision: CastContext

Date:

2026-07-10


## Context

Runtime objects were creating their own initial casting state.

Example:

ProjectileRuntimeObject created its own:

- position
- direction


This prevented spells from being created by different sources.


## Decision

Introduce CastContext as the input to SpellInstance.Cast().

CastContext contains:

- Owner
- Position
- Direction


## Consequences

Positive:

- Player, AI, turrets and other sources can use the same casting pipeline.
- Runtime objects no longer invent initial state.
- Future modifiers can use cast information.


Negative:

- Cast data must be explicitly passed through the pipeline.


---

# Decision: Behavior Spawn Capability

Date:

2026-07-10


## Context

Modules may need to request additional spell objects.

Example:

Fork projectile modifier.


Direct creation inside modules would break the architecture because modules would become coupled to runtime objects.


## Decision

Behaviors may expose optional capabilities through interfaces.

Current example:

ISpellSpawner

Implemented by:

ProjectileBehavior


Modules request creation through capabilities rather than creating runtime objects directly.


## Consequences

Positive:

- Behaviors keep ownership of existence.
- Modules remain generic.
- Runtime object creation remains centralized.


Negative:

- Additional capabilities may be required as more complex modifiers appear.


---

# Decision: CastEvent carries CastContext

Date:

2026-07-10


## Context

Modules reacting to casting need access to the original cast information.


## Decision

CastEvent contains:

- SpellInstance
- CastContext


## Consequences

Positive:

- Cast-time modifiers can react without global state.
- Future modules can use owner, position and direction.


Future consideration:

If cast modification becomes complex, introduce a separate modification phase instead of expanding CastEvent indefinitely.