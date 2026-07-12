# First Playable Prototype Roadmap

## Purpose

This roadmap defines the minimum development path from the current validated spell architecture to a first playable prototype.

The objective is not to build a complete game.

The objective is to validate two core questions:

1. Does the spell composition architecture remain robust when integrated into gameplay?
2. Is creating and modifying spells an engaging gameplay mechanic?

Each session represents one focused development milestone.

At the end of each session:

- Review completed work.
- Validate the intended gameplay or architectural goal.
- Update this document.
- Move to the next session only when the previous validation goal is satisfied.

---

# Current State

## Architecture Status

The spell composition architecture is validated.

Current flow:

SpellDefinition

↓

SpellConfiguration

↓

SpellFactory

↓

SpellInstance

↓

SpellBehavior

↓

SpellRuntimeObjects

↓

Runtime Events

↓

SpellModules

↓

Gameplay Systems

---

# Session Status

Completed:

- Session 1 - Player Spell Configuration
- Session 2 - Combat Foundation
- Session 3 - Gameplay Variety


Next:

- Session 4 - Progression Loop

---

# Session 1 - Player Spell Configuration

## Status

Completed.

## Validation Question

Can the player own and modify spell compositions independently from runtime spells?

Answer:

Yes.

---

# Session 2 - Combat Foundation

## Status

Completed.

## Validation Question

Can the existing spell architecture operate inside a real gameplay loop?

Answer:

Yes.

---

# Session 3 - Gameplay Variety

## Status

Completed.

---

## Goal

Introduce the minimum amount of content required to evaluate spell creativity.

The purpose was not content creation.

The purpose was creating meaningful choices.

---

## Implemented

### Spell Testing

Added:

- SpellTester composition switching
- Runtime SpellConfiguration creation


Test compositions:

Projectile + Fire

Projectile + Explosion

Projectile + Fire + Explosion

Projectile + Fire + Explosion + Size Modifier


---

### Combat Testing

Added:

- EnemyWaveSpawner
- Moving TargetDummy prototype


Purpose:

Create repeatable combat scenarios.

---

## Validation Results

### Projectile + Fire

Result:

- Strong single target damage
- Weak against groups


### Projectile + Explosion

Result:

- Strong area damage
- Weak against isolated enemies


### Projectile + Fire + Explosion

Result:

- Powerful hybrid composition
- Demonstrates emergent spell construction


---

## Validation Question

Does spell composition create meaningful gameplay choices?

Answer:

Yes.

---

# Session 4 - Progression Loop

## Status

Next.

## Goal

Validate whether improving spells is motivating.

Possible validation areas:

- Choosing new modules
- Improving existing modules
- Creating stronger spell variations
- Temporary run-based upgrades

Avoid adding progression systems without a gameplay validation purpose.

---

# Session 5 - First Playtest

## Goal

Create a stable repeatable prototype loop.

---

# Session 6 - Architecture and Gameplay Review

## Goal

Evaluate the prototype before expanding the game.