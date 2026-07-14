Create Spell
|
Combat Trial
|
Discovery
|
Experimentation
|
Advanced Trial
|
Guardian
|
Knowledge Gained
|
New Expedition


---

# Run Structure

The game should introduce the concept of a research expedition.

A run contains:

- Temporary spell evolution
- Room progression
- Research decisions
- Combat encounters
- Final challenge

The first implementation should not include procedural generation.

A fixed authored sequence is preferred.

Example:


Start Run

↓

Combat Room

↓

Research Room

↓

Combat Room

↓

Guardian Room

↓

Run Complete


---

# Room Types

## Combat Room

Purpose:

Test the player's current spell experiments.

Possible encounters:

- Enemy waves
- Elite enemies
- Combat challenges

Combat should evaluate spell creation choices.

---

## Research Room

Purpose:

Allow the player to modify existing experiments.

Research actions:

- Add module
- Remove module
- Replace module
- Transform module

Research modifies SpellConfiguration.

Research does not modify active SpellInstances.

---

## Anomaly Room

Future expansion.

Purpose:

Introduce risk and experimentation.

Possible concepts:

- Increase instability
- Gain unusual effects
- Accept unpredictable consequences

This system should not be implemented until the core research loop is validated.

---

## Guardian Room

Purpose:

Final test of the current experiment.

The guardian should challenge the player's created spell system rather than simply require higher damage.

---

# Spell Management

Baseline player loadout:

- 2 active spells
- 1 passive spell

The player maintains a small number of complex experiments instead of collecting many independent abilities.

---

# Spell Evolution

Preferred model:

The player edits existing spells.

A spell is not replaced by another item.

A spell is rewritten through experimentation.

Example:


Projectile

Fire

Explosion


can evolve into:


Projectile

Fire

Chain

Explosion


The identity comes from the composition.

---

# Research System Direction

Research choices should represent discoveries.

A reward is not simply:

"Gain stronger ability."

A reward represents:

"Understand a new magical principle."

Examples:

- Discover Fire interaction
- Discover Chain behavior
- Transform an existing principle
- Remove a limitation

The system should create new possibilities rather than only increase numerical power.

---

# Reward System Transition

Current prototype:


Encounter Complete

↓

Reward Offer

↓

Add Module


Future direction:


Encounter Complete

↓

Research Opportunity

↓

Experiment Choice

↓

Spell Modification

↓

Updated SpellConfiguration


The existing RewardController pattern remains valid.

The system should expand rewards into broader research actions instead of replacing the architecture.

---

# Knowledge Progression

Knowledge represents permanent understanding of magic.

It should unlock:

- New behaviors
- New modules
- New interactions

It should not provide:

- Permanent damage bonuses
- Character statistics
- RPG progression systems

The player improves through understanding magic, not through numerical growth.

---

# Complexity and Instability

These systems represent future pillars of spell experimentation.

They are intentionally deferred.

---

## Complexity

Potential purpose:

Define how much a spell can contain.

Possible future uses:

- Build limits
- Research requirements
- Spell identity rules

---

## Instability

Purpose:

Represent dangerous experimentation.

Instability should create decisions, not simple punishment.

Possible future direction:

Higher instability creates:

- Greater opportunities
- Strange interactions
- Dangerous consequences

Avoid implementing instability as only:

"Chance for spell failure."

---

# Implementation Constraints

## Procedural Tower

Deferred.

The first version should use authored room sequences.

The goal is validating the gameplay loop.

---

## Dynamic Spell Mutation

Deferred.

Prefer:

Modify future spell casts through SpellConfiguration.

Avoid requiring all active runtime objects to transform dynamically.

---

## Universal Interaction Systems

Deferred.

Prefer explicit interaction points.

Avoid attempting to generate every possible magical interaction automatically.

---

# Implementation Order

## Phase 1 - Run Foundation

Introduce:

- Run lifecycle
- Start run
- End run
- Reset temporary state

---

## Phase 2 - Room Progression

Introduce:

- Room sequence
- Combat room
- Research room
- Guardian room

No procedural generation.

---

## Phase 3 - Research Choices

Expand rewards into:

- Spell modifications
- Experiment choices
- Research opportunities

---

## Phase 4 - Knowledge

Introduce minimal permanent progression:

- Unlock discoveries
- Expand available principles

No power scaling.

---

# Open Questions

Still requiring design:

- Exact research room presentation
- Guardian philosophy
- Enemy roles
- Spell complexity rules
- Instability implementation
- Knowledge unlock structure
- Multiple spell management
- Passive spell design

---

# Success Criteria

The vertical slice succeeds if the player experience creates the feeling:

"I discovered a magical principle and created something that could not have existed before."

The system should encourage experimentation rather than optimization toward a single strongest build.