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

ProjectileBehavior

↓

ProjectileRuntimeObject

↓

HitEvent

↓

FireModule
ExplosionModule

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

      └── SpellEventBus

---

# Runtime Objects

Implemented:

- ProjectileRuntimeObject
- ExplosionRuntimeObject

Future:

- Aura
- Trap
- Minion
- Persistent Zone

---

# Behaviors

Implemented:

- ProjectileBehavior

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

SpellEventBus

Current:

- CastEvent
- HitEvent

GameEventBus

Current:

- DamageEvent

Future:

- HealEvent
- StatusAppliedEvent
- DeathEvent

---

# Gameplay Systems

Implemented:

- DamageSystem

Implemented interfaces:

- IDamageable

Current test implementation:

- TargetDummy

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

Runtime owns gameplay.

View owns Unity components.

---

# Proven Design Principles

✓ No individual spell classes

✓ Behaviors own existence

✓ Modules react through events

✓ Multiple modules react independently

✓ Modules can spawn runtime objects

✓ Runtime objects own gameplay

✓ Gameplay systems communicate through GameEventBus

✓ ScriptableObjects contain configuration only

---

# Current Limitation

Projectiles currently spawn at a hardcoded position and direction.

Next milestone:

Introduce CastContext to propagate:

- Owner
- Position
- Direction

through the cast pipeline.

Target flow:

SpellTester

↓

CastContext

↓

SpellInstance.Cast(context)

↓

Behavior.Cast(context)

↓

RuntimeObject

This will support player casting, enemies, turrets, targeted spells and future spell modifiers.