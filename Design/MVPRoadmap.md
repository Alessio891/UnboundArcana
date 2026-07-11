# First Playable Prototype Roadmap

## Purpose

This document defines the steps required to move Unbound Arcana from the current validated spell architecture into the first playable gameplay prototype.

The goal is not to create a complete game.

The goal is to validate the core gameplay loop:

```
Create Spell

↓

Enter Combat

↓

Fight Enemies

↓

Gain Rewards

↓

Improve Spell

↓

Continue Fighting
```

The prototype should prove that the spell composition system creates meaningful gameplay.

---

# Current State

## Completed Systems

The spell architecture has been validated.

Current flow:

```
SpellDefinition

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
```

---

## Spell System

Implemented:

### Behaviors

* ProjectileBehavior
* AuraBehavior
* BeamBehavior

Behaviors are responsible for:

* existence
* spawning
* movement
* lifetime
* runtime object creation

Behaviors do not know modules exist.

---

### Runtime Objects

Implemented:

* ProjectileRuntimeObject
* ExplosionRuntimeObject
* AuraRuntimeObject
* BeamRuntimeObject

Runtime objects own:

* gameplay state
* lifetime
* updates

Views only represent runtime objects.

---

### Modules

Implemented:

* FireModule
* ExplosionModule
* ForkModule
* SplitOnDestroyModule
* CastSpellOnDestroyModule
* SizeModifierModule

Modules:

* react through events
* extend behavior
* create effects
* contribute modifiers

Modules do not communicate directly.

---

### Events

Implemented:

Spell events:

* CastEvent
* HitEvent
* ProjectileSpawnedEvent
* ProjectileDestroyedEvent
* RuntimeObjectSpawnedEvent
* RuntimeObjectDestroyedEvent

Game events:

* DamageEvent

---

## Modifier System

Implemented foundation:

Stats:

* Damage
* Size
* Speed
* Duration

Modifiers support:

* Flat
* Percent
* Multiplier

Stats are aggregated at runtime.

Ownership:

```
Behavior

provides existence-related defaults


Module

provides gameplay contributions
```

The spell composition creates the final effective stats.

---

# Remaining Prototype Requirements

The next phase is gameplay validation.

---

# Phase 1 - Spell Loadout Layer

## Goal

Create the bridge between authored spell data and player-owned spell builds.

Currently:

```
SpellDefinition

↓

SpellInstance
```

The game requires:

```
SpellDefinition

↓

Player Spell Configuration

↓

SpellInstance
```

---

## New Concept

A player spell configuration represents the player's current build.

Example:

```
Projectile

+

Fire Module Level 3

+

Explosion Module Level 1
```

This is not a runtime object.

It is a configuration used to create runtime spells.

---

## Responsibilities

The loadout system should store:

* selected behavior
* selected modules
* module progression values
* future upgrade information

It should not contain:

* runtime objects
* views
* active spell state

---

# Phase 2 - Player Combat Prototype

## Goal

Validate that spells work in actual combat.

Required systems:

## Player

Minimum:

* movement
* aiming
* casting
* health
* death

The player should only provide:

```
CastContext

↓

SpellInstance.Cast()
```

The spell system should remain unaware of the player implementation.

---

## Enemy

Minimum:

* movement toward player
* health
* damage reception
* death

Required pipeline:

```
Spell

↓

Hit Event

↓

Module

↓

Damage Event

↓

Damage System

↓

Enemy Health

↓

Death
```

---

# Phase 3 - Basic Game Arena

## Goal

Create a repeatable combat scenario.

Required:

* player spawn
* enemy spawning
* combat area
* restart/reset

No need for:

* tower generation
* procedural rooms
* progression systems

A single arena is enough.

---

# Phase 4 - Reward System

## Goal

Validate the core spell-building fantasy.

After defeating enemies, the player receives choices.

Examples:

```
Add Fire Module

Add Explosion Module

Increase Size
```

Rewards modify the player spell configuration.

They should not directly modify active runtime spells.

---

Flow:

```
Reward

↓

Player Spell Configuration

↓

SpellFactory

↓

New SpellInstance
```

---

# Phase 5 - First Playable Loop

The minimum successful prototype is:

```
Start Run

↓

Choose Spell

↓

Fight Enemies

↓

Defeat Enemies

↓

Receive Upgrade

↓

Modify Spell

↓

Fight Stronger Enemies

```

At this point the game has validated:

* spell composition
* combat interaction
* progression choices
* player expression through builds

---

# Systems Intentionally Deferred

The following should not be implemented before the first playable state.

## Tower System

Deferred:

* rooms
* floors
* procedural generation
* bosses

## Inventory

Deferred:

* equipment
* item management
* shops

## Meta Progression

Deferred:

* unlocks
* hub
* permanent upgrades

## Advanced Combat

Deferred:

* status effects
* resistances
* complex AI
* elite enemies

---

# Important Architectural Constraints

The following rules remain unchanged.

## Spells are compositions

Never introduce:

```
FireballSpell

IceSpell

ExplosionSpell
```

Spells are always:

```
Behavior

+

Modules

+

Stats

+

Events
```

---

## Behaviors own existence

Behaviors control:

* spawning
* lifetime
* movement

They do not know effects exist.

---

## Modules extend through events

Modules react to events.

They do not directly modify other modules or behaviors.

---

## Runtime state stays runtime

ScriptableObjects contain configuration only.

Runtime objects contain gameplay state.

---

# Recommended Development Order

1. Implement Player Spell Configuration.
2. Connect player casting to SpellFactory.
3. Implement player controller.
4. Implement enemy health and death.
5. Validate damage pipeline.
6. Add arena spawning.
7. Add reward choices.
8. Playtest the first complete loop.

The priority is not adding more spell content.

The priority is proving that the spell composition system creates an enjoyable gameplay loop.
