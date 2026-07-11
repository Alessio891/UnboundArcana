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


Implemented:

## Behaviors

- ProjectileBehavior
- AuraBehavior
- BeamBehavior

## Runtime Objects

- ProjectileRuntimeObject
- ExplosionRuntimeObject
- AuraRuntimeObject
- BeamRuntimeObject

## Modules

- FireModule
- ExplosionModule
- ForkModule
- SplitOnDestroyModule
- CastSpellOnDestroyModule
- SizeModifierModule

## Systems

- SpellEventBus
- GameEventBus
- Runtime Stat Aggregation
- Runtime Modifier System
- Spell Chaining
- Runtime Context Injection
- SpellConfiguration
- Runtime Spell Lifecycle Cleanup

---

# Prototype Philosophy

The prototype should prioritize validation over content.

Do not add systems because they are expected in the final game.

Add systems only when they answer a gameplay question.

Priority order:

1. Spell ownership and creation
2. Combat interaction
3. Spell variety
4. Progression choices
5. Extended playtesting

---

# Session 1 - Player Spell Configuration

## Goal

Create the missing bridge between authored spell data and player-owned spell builds.

## Status

Completed.

---

## Implemented

The system now separates:

Player-owned spell builds

from

Runtime spell execution.

Flow:


SpellDefinition

↓

SpellConfiguration

↓

SpellFactory

↓

SpellInstance


`SpellConfiguration` represents the player's current spell composition.

It contains:

- Selected behavior
- Selected modules

It does not contain:

- Runtime objects
- Active gameplay state
- Views

---

## Runtime Spell Lifecycle

SpellInstances represent individual spell executions.

They are created when casting occurs.

Flow:


SpellConfiguration

↓

SpellFactory

↓

SpellInstance

↓

Runtime Objects

↓

Execution

↓

Completion

↓

SpellRuntimeManager Removal


---

## Validation Question

Can the player own and modify spell compositions independently from runtime spells?

Answer:

Yes.

---

## Completion Criteria

Completed:

- Create a player-owned spell build.
- Modify the build.
- Generate a SpellInstance through SpellFactory.
- Separate player spell ownership from runtime execution.
- Ensure runtime spell instances are cleaned up after completion.

---

# Session 2 - Combat Foundation

## Status

Next session.

---

## Goal

Connect the spell system to a playable combat scenario.

---

## Required Systems

### Player

- Movement
- Aiming
- Casting


### Enemy

- Movement
- Health
- Damage reception
- Death


### World

- Arena
- Basic enemy spawning

---

## Required Gameplay Flow


Player

↓

Cast Spell

↓

Spell Runtime Objects

↓

Hit Event

↓

Damage Event

↓

Enemy Health

↓

Death


---

## Scope Restrictions

Do not add:

- Complex AI
- Status effects
- Resistances
- Procedural generation
- Bosses

---

## Validation Question

Can the existing spell architecture operate inside a real gameplay loop?

---

## Completion Criteria

A player can:

- Move.
- Cast a spell.
- Damage an enemy.
- Defeat an enemy.

---

# Session 3 - Gameplay Variety

## Goal

Introduce the minimum amount of content required to evaluate spell creativity.

The purpose is not content creation.

The purpose is creating meaningful choices.

---

# Session 4 - Progression Loop

## Goal

Validate whether improving spells is motivating.

---

# Session 5 - First Playtest

## Goal

Create a stable repeatable prototype loop.

---

# Session 6 - Architecture and Gameplay Review

## Goal

Evaluate the prototype before expanding the game.