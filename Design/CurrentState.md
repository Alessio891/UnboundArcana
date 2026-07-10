# Current State

Last Updated:

2026-07-11

---

# Current Milestone

Behavior Expansion and Composition Validation

---

# Completed

## Aura Behavior

Implemented.

Purpose:

Validate that the architecture works without projectile assumptions.

Implemented:

- AuraBehavior
- AuraRuntimeObject
- AuraView


Validated:

- Persistent runtime objects
- Independent lifetime updates
- Non-projectile spell behavior


---

## Runtime Lifecycle Events

Completed.

Added:

RuntimeObjectDestroyedEvent


Purpose:

Allow modules to react to runtime object lifecycle without knowing behaviors.


Used by:

- Aura expiration
- Future destruction effects


---

## Spell Runtime Context

Completed.

Implemented:

- SpellRuntimeContext
- ISpellRuntime


Purpose:

Provide runtime services to spells without coupling them directly to scene objects.


Current services:

- GameEventBus
- Spell registration


---

## Deferred Spell Registration

Completed.

Purpose:

Allow spells to create other spells during updates safely.


Flow:

Register request

↓

Pending list

↓

Added after update


---

## Spell Chaining

Completed.

Implemented:

CastSpellOnDestroyModule


Validated:

- A spell can trigger another spell composition.
- Chained spells use the normal SpellFactory pipeline.
- Behaviors remain independent.


---

# Current Implemented Systems

## Behaviors

- ProjectileBehavior
- AuraBehavior


## Modules

- FireModule
- ExplosionModule
- ForkModule
- SplitOnDestroyModule
- CastSpellOnDestroyModule


## Runtime Objects

- ProjectileRuntimeObject
- ExplosionRuntimeObject
- AuraRuntimeObject


## Events

SpellEventBus:

- CastEvent
- HitEvent
- ProjectileSpawnedEvent
- ProjectileDestroyedEvent
- RuntimeObjectDestroyedEvent


GameEventBus:

- DamageEvent


## Gameplay

- DamageSystem
- IDamageable
- TargetDummy


---

# Current Architecture Status

The composition model is validated by:

- Multiple behavior types
- Multiple independent modules
- Runtime object lifecycle
- Module-created effects
- Spell-to-spell composition


---

# Next Objectives

Possible next steps:

## More Behaviors

Examples:

- Beam
- Trap
- Zone
- Summon
- Orbit


## More Modules

Examples:

- Homing
- Pierce
- Bounce
- Status effects
- Element conversion


## Performance Validation

Evaluate:

- Runtime object count
- Pooling
- Event allocations
- Large combat scenarios


Do not introduce new abstraction layers until required by concrete gameplay needs.