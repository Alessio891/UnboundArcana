# Architecture Snapshot

Last Updated:

2026-07-10

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

ProjectileBehavior

↓

ProjectileRuntimeObject

↓

Runtime Events

↓

FireModule
ExplosionModule
ForkModule
SplitOnDestroyModule

↓

DamageEvent

↓

DamageSystem

↓

IDamageable


---

# Current Ownership

SpellRuntimeManager

├── GameEventBus

└── SpellInstances

      ├── SpellBehavior

      ├── SpellModules

      ├── SpellRuntimeObjects

      ├── SpellEventBus

      └── Optional Behavior Capabilities

            └── ISpellSpawner


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

# Runtime Objects

Implemented:

- ProjectileRuntimeObject
- ExplosionRuntimeObject


Runtime object responsibilities:

- Maintain gameplay state
- Execute runtime updates
- Control lifetime


Runtime objects store creation information separately through SpawnContext when required.


Future:

- Aura
- Trap
- Minion
- Persistent Zone


---

# Behaviors

Implemented:

- ProjectileBehavior


Capabilities:

- ISpellSpawner


Future:

- Beam
- Aura
- Nova
- Trap
- Orbit
- Meteor


---

# Modules

Implemented:

- FireModule
- ExplosionModule
- ForkModule
- SplitOnDestroyModule


Current module responsibilities:

FireModule

- Reacts to HitEvent
- Creates DamageEvents


ExplosionModule

- Reacts to HitEvent
- Creates ExplosionRuntimeObjects


ForkModule

- Reacts to CastEvent
- Uses ISpellSpawner capability
- Requests additional projectile creation


SplitOnDestroyModule

- Reacts to ProjectileDestroyedEvent
- Requests additional projectile creation
- Uses SpawnContext rules to prevent recursive splitting


Future:

- Poison
- Ice
- Burn
- Pierce
- Bounce
- Split
- Homing
- Chain
- Lifesteal


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

- Destruction-based modifiers


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


Rules:

Runtime owns gameplay.

View owns Unity components.

Views do not create gameplay state.


---

# Spawn Capability Pattern

Behaviors may expose optional capabilities.


Current:

ProjectileBehavior

implements:

ISpellSpawner


Purpose:

Allow modules to request creation of runtime objects without knowing runtime implementations.


Example:

ForkModule

↓

ISpellSpawner

↓

ProjectileBehavior

↓

ProjectileRuntimeObject


Modules do not:

- instantiate runtime objects
- create views
- access prefabs
- manage lifecycle


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


Runtime objects may retain SpawnContext to determine future behavior.


---

# CastContext

Purpose:

Provide external casting information to the spell pipeline.


Current fields:

- Owner
- Position
- Direction


Used by:

- Player casting
- Future enemy casting
- Future turret casting
- Future targeted spells


Runtime objects no longer create their own initial state.


---

# Proven Design Principles

✓ No individual spell classes

✓ Behaviors own existence

✓ Modules react through events

✓ Modules do not communicate directly

✓ Multiple modules react independently

✓ Modules can request creation through behavior capabilities

✓ Runtime objects own gameplay state

✓ Gameplay systems communicate through GameEventBus

✓ ScriptableObjects contain configuration only

✓ Views only represent runtime objects

✓ Casting sources provide initial spell state

✓ Runtime lifecycle events allow independent modifiers

✓ Spawn metadata controls child object behavior


---

# Current Limitations

Current:

CastEvent is used for cast reactions.

Future consideration:

If cast modification becomes complex, introduce a separate cast modification phase instead of expanding CastEvent responsibilities.


SpawnContext currently supports simple modifier inheritance rules.

Future:

- Selective module inheritance
- Modified child compositions
- More advanced spawn rules


---

# Next Milestone

Behavior Expansion and Architecture Validation

Objectives:

- Implement additional behaviors:
    - Aura
    - Beam
    - Trap

- Verify that the composition model is behavior-independent.

- Evaluate whether new behaviors require:
    - additional capabilities
    - additional runtime events
    - new runtime object patterns


Avoid redesigning existing architecture unless a concrete limitation appears.