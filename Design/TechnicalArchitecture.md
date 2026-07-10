# Technical Architecture

## Engine

Unity 6000.3.19f1 LTS

Project type:

- 2D

Rendering:

- Built-in Render Pipeline

---

# Architecture Style

Data-driven composition.

Prefer:

- ScriptableObjects
- Composition
- Runtime objects
- Events
- Interfaces

Avoid:

- Deep inheritance
- Individual spell classes
- Runtime state inside ScriptableObjects

---

# Main Systems

- Spell System
- Character System
- Enemy System
- Combat System
- Room System
- Loot System
- UI System
- Save System

---

# Spell Runtime Architecture

## Editor Layer

SpellDefinition

Contains:

- SpellBehaviorDefinition
- SpellModuleDefinition[]
- Configuration data

ScriptableObjects are configuration only.

---

## Runtime Layer

SpellFactory

↓

SpellInstance

↓

SpellBehavior

↓

SpellRuntimeObjects

↓

Views (Unity GameObjects)

SpellInstance owns:

- Behavior
- Modules
- Runtime Objects
- SpellEventBus
- Reference to GameEventBus

---

# Behaviors

Behaviors define how a spell exists.

Responsibilities:

- spawning
- movement
- lifetime
- runtime object creation

Behaviors never know modules exist.

Examples:

- Projectile
- Beam
- Aura
- Trap
- Minion

---

# Modules

Modules extend behaviors.

Responsibilities:

- React to spell events
- Publish gameplay events
- Spawn additional runtime objects

Modules never communicate directly.

Examples:

- Fire
- Explosion
- Poison
- Pierce
- Split

---

# Runtime Objects

Runtime objects represent gameplay entities.

Examples:

- Projectile
- Explosion
- Aura
- Trap
- Minion

Lifecycle:

Initialize

↓

Tick

↓

Destroy

↓

Removed by SpellInstance

Runtime objects own gameplay state.

---

# Views

Views are MonoBehaviours representing runtime objects.

Example:

ProjectileRuntimeObject

↓

ProjectileView

Views never own gameplay logic.

---

# Event Architecture

Two event buses exist.

## SpellEventBus

Owned by each SpellInstance.

Purpose:

Internal communication inside a spell.

Current events:

- CastEvent
- HitEvent

Future events:

- SpawnEvent
- TickEvent
- ExpireEvent
- DestroyEvent

---

## GameEventBus

Owned by SpellRuntimeManager.

Purpose:

Communication between spells and gameplay systems.

Current events:

- DamageEvent

Future examples:

- HealEvent
- StatusAppliedEvent
- DeathEvent

---

# Gameplay Systems

Gameplay systems consume GameEventBus events.

Example:

DamageEvent

↓

DamageSystem

↓

IDamageable

Spells never manipulate gameplay entities directly.

---

# Spell Sandbox

Purpose:

Validate architecture before implementing the complete game loop.

Current goals:

- Test behaviors
- Test modules
- Test runtime objects
- Validate composition
- Validate event architecture

---

# Folder Structure

Assets/

Scripts/

UnboundArcana/

Core/

Spells/

Character/

Combat/

Enemy/

Rooms/

Loot/

UI/

ScriptableObjects/

Prefabs/

Art/

Audio/