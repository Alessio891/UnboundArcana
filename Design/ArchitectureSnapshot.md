# Architecture Snapshot

Last Updated:

2026-07-12

---

# Current Vertical Slice

Implemented:

SpellDefinition

↓

SpellConfiguration

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

---

# Current Ownership

SpellRuntimeManager

├── GameEventBus

├── Active SpellInstances

└── SpellRuntimeContext


SpellConfiguration

├── SpellBehaviorDefinition

└── SpellModuleDefinition[]


SpellInstance

├── SpellBehavior

├── SpellModules

├── SpellRuntimeObjects

├── SpellEventBus

├── SpellRuntimeContext

├── Runtime Stats

└── Optional Behavior Capabilities

---

# Spell Ownership Model

The player does not own active SpellInstances.

The player owns spell configurations.

Flow:

Player Spell Configuration

↓

SpellFactory

↓

SpellInstance

↓

Runtime Objects


SpellConfigurations represent editable spell builds.

SpellInstances represent temporary gameplay execution.

---

# Spell Casting Flow

Current:

Casting Source

↓

SpellConfiguration

Contains:

- Selected behavior
- Selected modules

↓

SpellFactory.Create()

↓

SpellInstance

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

# Runtime Spell Lifecycle

A SpellInstance exists only during execution.

Lifecycle:

Create

↓

Initialize

↓

Cast

↓

Runtime Objects Active

↓

Runtime Objects Complete

↓

Spell Finished

↓

SpellRuntimeManager Removes Instance


SpellRuntimeManager owns active runtime spell instances.

---

# Runtime Stats

Stats are represented by:

SpellStatCollection

The collection is owned by the SpellInstance runtime.

Stats are composed from spell components.

Flow:

SpellInstance

↓

SpellStatCollection

↑

Behavior

↑

Modules

---

# Stat Ownership

Behaviors provide default stats required for their existence.

Examples:

Projectile:

- Speed
- Duration


Aura:

- Duration


Modules provide gameplay contributions.

Examples:

FireModule:

- Damage


ExplosionModule:

- Damage
- Size
- Duration


The spell composition determines the final effective stats.

---

# Behaviors

Implemented:

## ProjectileBehavior

Creates:

- ProjectileRuntimeObject

Capabilities:

- ISpellSpawner

Provides base stats:

- Speed
- Duration


---

## AuraBehavior

Creates:

- AuraRuntimeObject

Purpose:

Validate persistent non-projectile behavior lifecycle.

Provides base stats:

- Duration


---

## BeamBehavior

Creates:

- BeamRuntimeObject

Purpose:

Validate active spell lifecycle control.

---

# Modules

Implemented:

## FireModule

- Reacts to HitEvent
- Creates DamageEvents
- Provides damage modifiers through runtime stats


---

## ExplosionModule

- Reacts to HitEvent
- Creates ExplosionRuntimeObjects
- Provides explosion-related stats


---

## ForkModule

- Reacts to CastEvent
- Uses ISpellSpawner capability


---

## SplitOnDestroyModule

- Reacts to ProjectileDestroyedEvent
- Uses SpawnContext rules


---

## CastSpellOnDestroyModule

- Reacts to RuntimeObjectDestroyedEvent
- Creates another SpellInstance


---

## SizeModifierModule

- Demonstrates runtime stat modifiers

---

# Modifier System

Implemented:

Stats:

- Damage
- Size
- Speed
- Duration


Modifier operations:

- Flat
- Percent
- Multiplier


Modifiers store:

- Stat
- Value
- Operation
- Source


The system aggregates modifiers instead of components directly modifying runtime object fields.

---

# Runtime Object Pattern

Runtime Object

↓

View


Runtime objects:

- Maintain gameplay state
- Query effective stats
- Control lifetime


Views:

- Represent Unity objects only

---

# Combat Integration

Validated:

Player

↓

Cast Spell

↓

Spell Runtime Objects

↓

Hit Event

↓

Spell Modules

↓

Damage Event

↓

Damage System

↓

Damage Receiver

↓

Enemy Death


Combat systems remain separated from spell execution.

The spell system creates gameplay events.

Gameplay systems consume those events.

---

# Prototype Entity Structure

Current prototype uses minimal components.

Implemented:

TargetDummy

Responsibilities:

- Receive damage
- Track health
- Handle death
- Move toward player for combat testing


The project does not currently contain a generic entity framework.

This remains intentional.

---

# Session 3 Validation Tools

Implemented:

## SpellTester

Purpose:

Create temporary SpellConfigurations for composition testing.

Current test compositions:

- Projectile + Fire
- Projectile + Explosion
- Projectile + Fire + Explosion + Size Modifier


The tester validates spell composition without introducing a loadout system.

---

## EnemyWaveSpawner

Purpose:

Provide repeatable combat pressure.

Responsibilities:

- Spawn TargetDummy enemies
- Control spawn interval
- Control maximum active enemies
- Position enemies around the player


The spawner does not know about spells or combat logic.

---

# Session 3 Validation Result

Validated:

Different spell compositions create different gameplay identities.

Tested:

## Projectile + Fire

Result:

- Strong single target damage
- Less effective against groups


## Projectile + Explosion

Result:

- Strong area damage
- Less effective against isolated targets


## Projectile + Fire + Explosion

Result:

- Powerful hybrid composition
- Demonstrates emergent spell combinations


---

# Proven Design Principles

✓ No individual spell classes

✓ Behaviors own existence

✓ Modules react through events

✓ Modules do not communicate directly

✓ Runtime objects own gameplay state

✓ ScriptableObjects contain configuration only

✓ Views only represent runtime objects

✓ Stats are composed from behavior and module contributions

✓ Modifier sources remain identifiable

✓ Spawn metadata controls child behavior

✓ Spells can compose other spells

✓ Player spell configuration is separated from runtime execution

✓ Runtime spell instances are disposable

✓ Combat systems consume game events instead of depending on spells

✓ Spell compositions create distinct gameplay outcomes

---

# Current Limitations

Current:

- Spell loadout system does not exist
- Module progression state is not defined
- Modifier stacking order is insertion based
- Enemy system is only a prototype target implementation
- No arena/game loop exists
- No progression or reward systems exist


Future:

- Progression validation
- Spell improvement choices
- Advanced modifier inheritance rules
- More composition experiments

---

# Next Milestone

Session 4 - Progression Loop

Objectives:

- Validate whether improving spells is motivating.
- Introduce progression choices only if they support spell creativity.