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
- Damage Pipeline
- Enemy Damage Reception
- Enemy Death


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


## Goal

Connect the spell system to a playable combat scenario.


---

## Implemented

### Player

Validated:

- Movement
- Aiming
- Casting


### Combat

Implemented:

- DamageEvent
- DamageSystem
- DamageInfo
- IDamageable


### Enemy Prototype

Implemented:

TargetDummy

Supports:

- Health
- Damage reception
- Death


---

## Validation Question

Can the existing spell architecture operate inside a real gameplay loop?

Answer:

Yes.


---

## Completion Criteria

Completed:

- Player can move.
- Player can cast a spell.
- Spell runtime objects execute.
- Spells generate hit events.
- Modules generate damage events.
- Enemies receive damage.
- Enemies can be defeated.


---

# Session 3 - Gameplay Variety

## Status

Next session.


---

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