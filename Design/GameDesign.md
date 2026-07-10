# Unbound Arcana - Game Design Document

## High Concept

Unbound Arcana is a 2D top-down ARPG roguelike focused on creating unique, procedural spells.

The player is a customizable mage who climbs an infinite tower, discovering new magical abilities and combining them into increasingly powerful and unusual spells.

The core fantasy is:

"Create a spell nobody else could have imagined."

The game is not about collecting predefined spells. It is about constructing a magical system through combinations.

---

# Gameplay Pillars

## 1. Spell Creativity

The primary gameplay mechanic is building spells from:

* Base behaviors
* Modules
* Stats
* Events
* Fusion

Players should be able to create:

* extremely powerful builds
* strange experimental builds
* weak but interesting builds

The player's identity comes from the magic they create.

---

## 2. Fast ARPG Combat

Combat should feel:

* fast
* responsive
* skill-based
* chaotic

Reference points:

* Hades
* Noita
* Risk of Rain
* Magicka
* Tiny Rogues

---

## 3. Roguelike Progression

Each run is temporary.

During a run:

* discover modules
* improve spells
* adapt builds

Between runs:

* unlock new possibilities
* expand the spell pool
* unlock systems

---

# Gameplay Loop

Hub / Preparation

↓

Customize character and spells

↓

Enter Tower

↓

Obtain spells and modules

↓

Modify spell builds

↓

Fight stronger enemies

↓

Boss encounter

↓

New biome

↓

Continue climbing

↓

Death / Completion

↓

Return to Hub

---

# Spell Philosophy

A spell is not a predefined ability.

A spell is a composition of:

* Behavior
* Modules
* Stats
* Events

Examples:

Projectile + Fire + Explosion

creates:

An explosive fire projectile

Projectile + Ice + Split

creates:

A splitting freezing projectile

---

# Development Philosophy

The game should prioritize:

1. Spell system
2. Combat feel
3. Procedural combinations
4. Content expansion

The spell system is the main feature.

Everything else exists to support it.

---

# Current Validation

The spell composition system has validated:

* Behaviors define spell existence
* Modules extend behavior through events
* Runtime objects contain gameplay state
* Spawned objects can carry composition rules
* Different runtime events can drive different spell interactions

Future systems should continue expanding the composition model rather than introducing predefined spell classes.