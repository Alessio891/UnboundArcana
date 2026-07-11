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

The previous SpellStats class was removed.

Stats are now represented by:

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
- Provides damage modifiers


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


The project does not currently contain a generic entity framework.

This remains intentional.


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


---

# Current Limitations

Current:

- Spell loadout system does not exist.
- Module progression state is not defined.
- Modifier stacking order is insertion based.
- Enemy system is only a prototype target implementation.
- No enemy spawning system exists.
- No arena/game loop exists.


Future:

- Spell variety validation
- Enemy spawning
- Rewards
- Progression
- Advanced modifier inheritance rules


---

# Next Milestone

Session 3 - Gameplay Variety

Objectives:

- Introduce enough spell combinations to evaluate creativity.
- Validate meaningful spell construction choices.
- Avoid content expansion without gameplay purpose.