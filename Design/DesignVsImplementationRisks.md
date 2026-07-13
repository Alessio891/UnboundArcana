# DesignVsImplementationRisk.md

# Unbound Arcana - Design vs Implementation Risk

## Purpose

This document exists to prevent the project from growing beyond realistic development limits.

Unbound Arcana is based around systemic spell creation.

Systemic games can become extremely expensive because complexity grows from interactions between systems.

The goal is not to remove ambition.

The goal is to identify:

* Core vision
* Safe implementation paths
* High-risk ideas
* Features that should be deferred

---

# Core Vision

These ideas define Unbound Arcana and should be protected.

## Spell Composition

The player creates spells by combining:

* Behaviors
* Principles
* Runtime interactions

Spells should feel authored by the player.

---

## Deterministic Magic

The player should understand the magical system.

A combination should have predictable results.

Discovery should come from:

* Learning combinations
* Understanding interactions
* Applying knowledge

Not from random spell behavior.

---

## Spell Evolution

Spells should evolve during runs.

The player should modify experiments rather than simply replacing equipment.

---

## Knowledge Progression

Permanent progression should expand possibilities.

Prefer:

* New behaviors
* New principles
* New interactions

Avoid:

* Permanent damage bonuses
* Pure numerical upgrades

---

# Safe Initial Implementation

## Spell Slots

Recommended baseline:

* 2 active spells
* 1 passive spell

Additional slots should be rare.

Complexity:
Low

Risk:
Low

---

## Checkpoint-Based Research

Preferred structure:

```
Combat Trial
        |
Discovery
        |
Experimentation
        |
Harder Trial
```

Advantages:

* Easier UI
* Easier balancing
* Cleaner state management

Complexity:
Medium

Risk:
Low

---

## Authored Behaviors

Behaviors should expose known capabilities.

Examples:

Projectile:

* Creation
* Movement
* Hit
* Destruction

Beam:

* Length
* Duration
* Tick effects

Principles modify available capabilities.

This avoids every combination becoming a custom implementation.

Complexity:
Medium

Risk:
Medium

---

# High Risk Systems

## Fully Procedural Research Branches

Original idea:

The Tower generates completely new magical research paths.

Problem:

Requires procedural solutions for:

* Enemies
* Rewards
* Rooms
* Visual identity
* Balance

Recommended approach:

Use authored research domains with procedural combinations.

Example:

Thermal + Fragmentation

rather than generating a completely new branch.

Risk:
High

---

## Fully Emergent Spell Interactions

Danger:

Allowing every principle to affect every other principle.

Example problems:

* Order of operations
* Unexpected exploits
* Difficult balancing
* Hard debugging

Recommended approach:

Interactions should happen through explicit capability points.

Risk:
Very High

---

## Per-Cast Wild Magic

The fantasy is attractive:

Every cast may produce unstable consequences.

Risk:

The player loses trust in their own creation.

Recommended approach:

Instability creates controlled events.

Examples:

* Extra effects
* Anomalies
* Environmental changes
* Temporary distortions

The spell remains understandable.

Risk:
High

---

# Instability Design Direction

Instability should represent:

* Power
* Complexity
* Loss of control

It should not simply be punishment.

Possible model:

Spell complexity increases instability.

Higher instability increases the chance of special events.

Events should be:

* Categorized
* Weighted
* Understandable

The player should think:

"I am pushing this experiment."

Not:

"The game randomly punished me."

---

# Procedural Content Direction

Prefer:

Procedural assembly.

Examples:

* Authored rooms
* Variable order
* Different research influences
* Different encounters

Avoid:

Procedural creation of entirely new content.

---

# First Complete Version Target

A realistic first full version should aim for:

## Spell System

* Several behaviors
* Several principles
* Multiple spell slots
* Deterministic combinations
* Controlled instability

---

## Tower

* Authored research domains
* Procedurally assembled rooms
* Research checkpoints

---

## Progression

* Knowledge unlocks
* New possibilities
* No endless power scaling

---

# Deferred Ambitions

Possible future expansions:

* More complex instability
* Dynamic research generation
* Advanced spell mutation
* More complex Tower simulation
* Deeper runtime interactions

These should only be attempted after the core game is successful.
