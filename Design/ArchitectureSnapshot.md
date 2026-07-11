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

├── Runtime Stats

└── Optional Behavior Capabilities


---

# Spell Casting Flow

Current:

Casting Source

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

# Runtime Stats

The previous SpellStats class was removed.

Stats are now represented by:

StatCollection

The collection is owned by the SpellInstance runtime.

Stats are composed from spell components.

Flow:

SpellInstance

↓

StatCollection

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


The system aggregates modifiers instead of components directly modifying runtime values.


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


---

# Current Limitations

Current:

- Spell loadout system does not exist.
- Player-owned spell composition does not exist.
- Module progression state is not defined.
- Modifier stacking order is insertion based.
- Spell cleanup/removal from SpellRuntimeManager is not implemented.


Future:

- Player Spell Configuration
- Upgrade system
- Reward system
- Combat prototype
- Advanced modifier inheritance rules


---

# Next Milestone

First playable prototype.

Objectives:

- Player casting
- Enemy combat
- Damage loop
- Spell loadouts
- Rewards
- Basic progression

Avoid adding content before validating the complete gameplay loop.