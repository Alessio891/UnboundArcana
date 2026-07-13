# Decision: Spell Runtime Lifecycle Ownership

Date:

2026-07-12

## Context

Originally SpellInstances were treated as reusable runtime versions of a spell.

This created a problem:

Player-owned spell configuration and runtime gameplay state were mixed together.

A spell configuration can change over time.

A runtime spell must represent one execution.

## Decision

SpellConfigurations own spell composition.

SpellInstances represent temporary execution.

Flow:

SpellConfiguration

↓

SpellFactory

↓

SpellInstance

↓

Runtime Objects


SpellInstances are created per cast.

## Consequences

Positive:

- Runtime state cannot leak between casts.
- Spell modifications affect future casts only.
- Multiple independent casts are possible.
- Player ownership is separated from gameplay execution.

Negative:

- More runtime allocation occurs.
- Persistent spell behaviors require explicit lifecycle handling.


---

# Decision: Behaviors Own Spell Identity

Date:

2026-07-13

## Context

During module expansion, a question emerged:

Should modules create new gameplay concepts, or should behaviors define the fundamental type of spell?

Examples:

- Projectile
- Beam
- Aura

Some possible future mechanics could technically be implemented either as behaviors or modules.

## Decision

Behaviors define the fundamental identity of a spell.

Modules enhance existing behaviors.

Examples:

ProjectileBehavior:

Creates projectile runtime objects.

Modules can modify:

- Movement
- Targeting
- Splitting
- On-hit behavior
- Destruction behavior

AuraBehavior:

Creates aura runtime objects.

Modules can modify:

- Radius
- Duration
- Interactions
- Effects

Modules should not replace behaviors.

## Consequences

Positive:

- Spell composition remains understandable.
- Player choices remain focused around modifying existing spell identities.
- The number of fundamental spell systems remains controlled.
- Avoids creating hundreds of specialized behaviors.

Negative:

- Some complex mechanics require deciding whether they belong to behavior or module.
- Future behavior boundaries must be designed carefully.


---

# Decision: Modules Should Prefer Capability-Based Modification

Date:

2026-07-13

## Context

Initial module ideas were often implemented around specific runtime objects.

Example:

Projectile modifier.

This raised a concern:

Would the architecture become locked into projectiles?

Not all spells are projectiles.

Future spells may include:

- Auras
- Beams
- Persistent areas
- Other runtime objects

## Decision

Modules should modify runtime objects based on capabilities rather than specific object types whenever possible.

Examples:

A movement modifier should affect objects that expose movement capability.

A targeting modifier should affect objects that expose targeting capability.

A lifetime modifier should affect objects with a lifetime.

The module should not assume that every spell is a projectile.

## Consequences

Positive:

- More generic architecture.
- Easier future expansion.
- Avoids projectile-specific dead ends.

Negative:

- Requires clearer runtime object contracts.
- Capability interfaces must be designed carefully.


---

# Decision: Runtime Object Modifiers Are Not Limited To Projectiles

Date:

2026-07-13

## Context

The first implementation examples involved projectile modifiers:

- Homing
- Chain
- Speed modification

The concern was that this could create a projectile-only architecture.

## Decision

Runtime object modification is considered a general extension point.

Projectile modifiers are only the first application.

Future examples:

Projectile:

- Homing
- Split
- Chain
- Movement changes

Aura:

- Size changes
- Duration changes
- Environmental interactions

Beam:

- Width changes
- Duration changes
- Targeting changes

## Consequences

Positive:

- Keeps the module system flexible.
- Allows emergent combinations.

Negative:

- Requires avoiding unnecessary abstraction before use cases exist.


---

# Decision: Aura Is A Behavior Concept, Not A Status Concept

Date:

2026-07-13

## Context

A possible implementation was discussed:

Creating a module that applies an aura to targets.

The term "Aura" created ambiguity because it could mean:

- A spell surrounding the player.
- A status effect applied to an entity.

## Decision

Aura currently refers to a spell behavior.

An AuraBehavior creates a persistent runtime object associated with the caster or world.

It is not a buff/debuff system.

Future status effects should be designed separately.

## Consequences

Positive:

- Keeps terminology clear.
- Avoids prematurely creating a status effect framework.
- Preserves the distinction between spell objects and entity effects.

Negative:

- Some future mechanics may require interaction between aura objects and status systems.


---

# Decision: Avoid Early Synergy Systems

Date:

2026-07-13

## Context

During MVP review, module synergy systems were considered.

A concern was raised:

Explicit synergy systems could conflict with player-driven spell creation.

Example:

Fire + Ice automatically creating a special combination.

This could shift the game toward predefined recipes.

## Decision

Do not introduce explicit synergy systems at this stage.

Emergent combinations should come from:

- Behaviors
- Modules
- Runtime interactions
- Player choices

Future compatibility rules may exist, but they should not replace player experimentation.

## Consequences

Positive:

- Maintains player-driven builds.
- Reduces design complexity.
- Avoids hidden recipes becoming mandatory.

Negative:

- Some combinations may require additional guidance for players.
- Build readability may become a challenge.


---

# Decision: Tags Should Be Introduced After More Module Diversity

Date:

2026-07-13

## Context

Gameplay tags were discussed as a possible solution for:

- Module compatibility
- Restrictions
- Build rules
- Runtime targeting

Example:

"Movement Modifier"

Allowing only one movement modifier.

## Decision

Do not introduce tags before enough module diversity exists.

The current system should first establish:

- More module variety
- Clear module categories
- Real examples of conflicts

Tags should solve actual design problems, not create restrictions prematurely.

## Consequences

Positive:

- Avoids unnecessary complexity.
- Allows tags to emerge from real requirements.

Negative:

- Some temporary module rules may remain manual until then.


---

# Decision: MVP Scope Completion

Date:

2026-07-13

## Context

The first playable prototype successfully validated:

1. Spell architecture integration into gameplay.
2. Player-driven spell modification as a gameplay mechanic.

The remaining problems are no longer architectural validation problems.

They are game design problems.

## Decision

The MVP milestone is considered complete.

The next phase focuses on transforming the validated prototype into a complete game direction.

Main areas:

- Gameplay loop
- Progression
- Presentation
- Build rules
- Content design
- Player experience

## Consequences

Positive:

- Stops expanding prototype systems indefinitely.
- Allows deeper design work.
- Prevents adding content without purpose.

Negative:

- More design decisions are now required before implementation.