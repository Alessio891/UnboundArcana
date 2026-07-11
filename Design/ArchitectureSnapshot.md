# Architecture Snapshot

Last Updated:

2026-07-11

---

# Current Vertical Slice

Implemented:

SpellDefinition

↓

SpellFactory

↓

SpellInstance

↓

CastContext

↓

SpellBehavior

↓

SpellRuntimeObjects

↓

Runtime Events

↓

SpellModules

↓

Game Events

↓

Gameplay Systems


Validated behaviors:

- ProjectileBehavior
- AuraBehavior
- BeamBehavior


Validated modules:

- FireModule
- ExplosionModule
- ForkModule
- SplitOnDestroyModule
- CastSpellOnDestroyModule


---

# Current Ownership

SpellRuntimeManager

├── GameEventBus

├── Active SpellInstances

└── SpellRuntimeContext


SpellInstance

├── SpellBehavior

├── SpellModules

├── SpellRuntimeObjects

├── SpellEventBus

├── SpellRuntimeContext

└── Optional Behavior Capabilities


---

# Spell Casting Flow

Current:

Input / Gameplay Source

↓

CastContext

Contains:

- Owner
- Position
- Direction

↓

SpellInstance.Cast(context)

↓

CastEvent

Contains:

- SpellInstance
- CastContext

↓

SpellBehavior.Cast(context)

↓

SpellRuntimeObjects
---

# Active Spell Control Flow

Some spells require continuous external control after casting.

Current lifecycle:

Casting source

↓

SpellInstance.Cast(context)

↓

SpellBehavior.Cast(context)


While active:

Casting source

↓

SpellInstance.UpdateCast(context)

↓

SpellBehavior.UpdateCast(context)


When control ends:

Casting source

↓

SpellInstance.End()

↓

SpellBehavior.End()


Examples:

Beam:

- Cast creates the beam
- UpdateCast changes aim direction
- End destroys the beam


Future examples:

- Guided projectiles
- Charging spells
- Maintained effects

---

# Runtime Context

Purpose:

Provide runtime services required by spells without coupling spells to Unity systems.


Current services:

- GameEventBus
- Spell registration through ISpellRuntime


Flow:

SpellRuntimeManager

↓

SpellRuntimeContext

↓

SpellInstance


---

# Runtime Objects

Implemented:

- ProjectileRuntimeObject
- ExplosionRuntimeObject
- AuraRuntimeObject


Runtime object responsibilities:

- Maintain gameplay state
- Execute runtime updates
- Control lifetime


Runtime objects may store creation information separately through SpawnContext when required.


---

# Behaviors

Implemented:

## ProjectileBehavior

Creates:

- ProjectileRuntimeObject


Capabilities:

- ISpellSpawner


## AuraBehavior

Creates:

- AuraRuntimeObject


Purpose:

Validate persistent non-projectile behavior lifecycle.

## BeamBehavior

Creates:

- BeamRuntimeObject


Purpose:

Validate active spell lifecycle control.


Validated:

- Runtime object creation
- Persistent runtime object lifecycle
- External direction updates
- Explicit spell ending through caster control

Future:

- Trap
- Minion
- Orbit
- Meteor


---

# Modules

Implemented:

## FireModule

- Reacts to HitEvent
- Creates DamageEvents


## ExplosionModule

- Reacts to HitEvent
- Creates ExplosionRuntimeObjects


## ForkModule

- Reacts to CastEvent
- Uses ISpellSpawner capability
- Requests additional projectile creation


## SplitOnDestroyModule

- Reacts to ProjectileDestroyedEvent
- Requests additional projectile creation
- Uses SpawnContext rules


## CastSpellOnDestroyModule

- Reacts to RuntimeObjectDestroyedEvent
- Creates another SpellInstance
- Registers and casts the chained spell


---

# Spell Chaining

Validated:

A spell can create another spell composition.

Flow:

SpellInstance A

↓

Module trigger

↓

SpellFactory

↓

SpellInstance B

↓

SpellRuntimeManager


Important:

The chained spell is a full spell composition.

It is not a special projectile or effect.


---

# Event Architecture

## SpellEventBus

Owned by:

SpellInstance


Current events:

## CastEvent

Contains:

- SpellInstance
- CastContext


Used for:

- Cast reactions
- Cast-time modifiers


## HitEvent

Contains:

- Runtime object source
- Hit position
- Target
- Owner


Used for:

- On-hit effects


## ProjectileSpawnedEvent

Contains:

- ProjectileRuntimeObject


Used for:

- Projectile lifecycle reactions


## ProjectileDestroyedEvent

Contains:

- ProjectileRuntimeObject


Used for:

- Projectile-specific destruction modifiers


## RuntimeObjectDestroyedEvent

Contains:

- SpellRuntimeObject


Used for:

- Generic runtime lifecycle reactions

Examples:

- Aura expiration
- Chained spells
- Future expiration effects


---

# GameEventBus

Owned by:

SpellRuntimeManager


Current events:

## DamageEvent

Used by:

- FireModule
- ExplosionModule


Future:

- HealEvent
- StatusAppliedEvent
- DeathEvent


---

# Runtime Object Pattern

Runtime Object

↓

View


Examples:

ProjectileRuntimeObject

↓

ProjectileView


ExplosionRuntimeObject

↓

ExplosionView


AuraRuntimeObject

↓

AuraView


Rules:

Runtime owns gameplay.

View owns Unity representation.

Views do not create gameplay state.


---

# Spawn Capability Pattern

Behaviors may expose optional capabilities.


Current:

ProjectileBehavior

implements:

ISpellSpawner


Purpose:

Allow modules to request behavior-owned creation.


Example:

ForkModule

↓

ISpellSpawner

↓

ProjectileBehavior

↓

ProjectileRuntimeObject


---

# Module Creation Rules

Current validated rule:

Modules do not create objects belonging to another behavior.

Modules may create runtime objects for effects they own.

Examples:

Allowed:

ExplosionModule

↓

ExplosionRuntimeObject


Not allowed:

SplitModule

↓

ProjectileRuntimeObject


because projectile creation belongs to ProjectileBehavior.


---

# SpawnContext

Purpose:

Provide creation metadata when runtime objects are spawned.


Current fields:

- Position
- Direction
- Modifier propagation rules


Used by:

- ProjectileBehavior
- Spawn-based modifiers


---

# CastContext

Purpose:

Provide external casting information.


Current fields:

- Owner
- Position
- Direction


Used by:

- Player casting
- Enemy casting
- Future casting sources


---

# Proven Design Principles

✓ No individual spell classes

✓ Behaviors own existence

✓ Modules react through events

✓ Modules do not communicate directly

✓ Runtime objects own gameplay state

✓ ScriptableObjects contain configuration only

✓ Views only represent runtime objects

✓ Casting sources provide initial spell state

✓ Runtime lifecycle events allow independent modifiers

✓ Spawn metadata controls child behavior

✓ Spells can compose other spells

✓ Runtime systems are injected through context


---

# Current Limitations

Current:

- CastContext is minimal.
- RuntimeObjectDestroyedEvent only exposes the destroyed object.
- Spell cleanup/removal from SpellRuntimeManager is not implemented yet.
- Module lifecycle interaction with active spell control is not defined yet.

Future:

- More lifecycle events
- More advanced cast context
- Spell lifetime management
- Runtime object pooling
- Module interaction with active spell control


---

# Next Milestone

Gameplay Expansion

Objectives:

- Add more behaviors:
    - Beam
    - Trap
    - Zone
    - Summon

- Add more gameplay modules:
    - Pierce
    - Bounce
    - Homing
    - Status effects

- Evaluate performance requirements:
    - Object pooling
    - Event allocation
    - Runtime object scaling


Avoid redesigning architecture unless a concrete gameplay limitation appears.