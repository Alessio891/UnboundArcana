# Current State

Last Updated:

2026-07-10

---

# Current Milestone

Cast Context and Modifier Foundation

---

# Completed

## CastContext

Implemented.

Purpose:

Move initial spell state from runtime objects into the casting pipeline.

Contains:

- Owner
- Position
- Direction

Current flow:

Cast source

↓

CastContext

↓

SpellInstance.Cast(context)

↓

Behavior

↓

Runtime Object

---

## Projectile Initialization

Completed.

ProjectileRuntimeObject no longer creates default values:

Before:

- Position = Vector3.zero
- Direction = Vector3.right

After:

State is provided during casting.

---

## CastEvent Context

Completed.

CastEvent now carries:

- SpellInstance
- CastContext

This allows modules to react to casting information.

---

## Behavior Spawn Capability

Completed.

Implemented:

ISpellSpawner

Current implementation:

ProjectileBehavior

Purpose:

Allow modules to request object creation without knowing runtime implementations.

---

## Fork Modifier

Completed.

Implemented:

ForkModule

ForkModuleDefinition

Behavior:

- Reacts to CastEvent
- Calculates additional directions
- Requests projectile creation through ISpellSpawner

Validated:

- Modules can extend spells
- Behaviors retain creation ownership
- Runtime objects remain independent

---

# Current Implemented Systems

## Behaviors

- ProjectileBehavior


## Modules

- FireModule
- ExplosionModule
- ForkModule


## Runtime Objects

- ProjectileRuntimeObject
- ExplosionRuntimeObject


## Events

SpellEventBus:

- CastEvent
- HitEvent

GameEventBus:

- DamageEvent


## Gameplay

- DamageSystem
- IDamageable
- TargetDummy

---

# Current Architecture Status

The composition model is validated by:

- Multiple modules reacting independently
- Modules extending behavior without inheritance
- Runtime object creation controlled by behaviors
- External casting data flowing through CastContext

---

# Next Objectives

Possible next steps:

## Projectile Modifiers

Examples:

- Homing
- Pierce
- Bounce
- Split on hit

## More Behaviors

Examples:

- Beam
- Aura
- Trap
- Orbit

## Spawn/Event Refinement

Evaluate whether future modifiers require:

- dedicated spawn events
- cast modification phase
- richer runtime object creation capabilities

Do not introduce new abstraction layers until required by a concrete feature.