# Status System

## Purpose

Statuses exist to create interactions between spells.

They are not intended to become a generic ability framework.


---

# Examples

Burning:

- deals damage over time
- can be consumed by explosions


Frozen:

- slows movement
- can be shattered by fire


Conductive:

- improves lightning interactions


Poison:

- accumulates stacks
- explodes at threshold


---

# Runtime Design

StatusDefinition:

ScriptableObject

Contains:

- configuration
- icon
- duration
- max stacks


StatusInstance:

Runtime object.

Contains:

- current duration
- current stacks
- entity reference


---

# Status Effects

Statuses may:

- subscribe to entity events
- apply stat modifiers
- trigger damage
- spawn visual effects


---

# Permanent Statuses

Statuses can also represent permanent effects.

Example:

A passive item granting:

+10% movement speed


A permanent status:

- has no expiration
- applies modifiers permanently
- can still be removed by source

