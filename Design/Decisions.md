# Decision: Spell Stat Ownership

Date:

2026-07-11

## Context

The original SpellStats class stored stats inside SpellDefinition.

This created an ownership problem.

A spell container does not know which stats are required.

Examples:

Projectile requires:

- Speed

Aura requires:

- Duration

Explosion requires:

- Size


## Decision

Stats are owned by SpellInstance runtime.

Behaviors and modules contribute stats.

Flow:

SpellInstance

↓

StatCollection

↑

Behavior

↑

Module


## Consequences

Positive:

- Components own their requirements.
- Different behaviors can use different stats.
- Modules can introduce new gameplay values.
- Future systems can contribute modifiers.

Negative:

- Stat discovery becomes dynamic.
- Editor tools may eventually be needed to display final values.