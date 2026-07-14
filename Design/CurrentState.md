# Current State

Last Updated:

2026-07-13

---

# Current Milestone

MVP Spell Composition Prototype

Completed.

---

# Milestone Overview

The first playable prototype milestone has been completed.

The purpose of this milestone was to validate:

1. Does the spell composition architecture remain robust when integrated into gameplay?
2. Is creating and modifying spells an engaging gameplay mechanic?

Both questions have been positively validated.

The prototype is now moving from architecture validation into deeper game design and production planning.

---

# Completed

## Spell Composition Architecture

Validated.

The system supports:

- Multiple behavior types
- Independent modules
- Runtime lifecycle events
- Spell chaining
- Runtime context injection
- Runtime object creation
- Modifier aggregation
- Runtime object modification
- Player-driven spell composition

Current architecture:

SpellDefinition

↓

SpellConfiguration

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

↓

Run Progression

---

# Player Spell Configuration

Implemented.

The system separates:

Player-owned spell builds

from

Runtime spell execution.

Flow:

SpellDefinition

↓

SpellConfiguration

↓

SpellFactory

↓

SpellInstance

---

SpellConfigurations represent editable spell builds.

SpellInstances represent temporary spell executions.

Changing a configuration affects future casts only.

Existing runtime spells remain independent.

---

# Runtime Spell Lifecycle

Implemented.

SpellInstances are created per cast.

They are not persistent player objects.

Flow:

Cast

↓

Create SpellInstance

↓

Initialize Behavior and Modules

↓

Create Runtime Objects

↓

Execute

↓

Finish

↓

Remove Runtime Instance

---

# Progression Loop Prototype

Completed.

Validated gameplay loop:

Encounter

↓

Combat

↓

Encounter Completion

↓

Reward Selection

↓

Modify Spell Configuration

↓

Continue Run

---

# Reward System

Implemented and improved.

Added:

- RewardController
- Random module offers
- Module acquisition
- Reward filtering
- Module category support

Rewards modify the player spell configuration.

They do not directly modify active spells.

---

# Combat Prototype

Implemented.

## Combat Interaction

Added:

- Player health
- Enemy health
- Enemy contact damage
- Enemy defeat handling
- Damage event pipeline

Damage flow:

DamageEvent

↓

Damage System

↓

IDamageable

---

## Player Casting

Implemented:

- Basic casting
- Cooldown handling

Current cooldown exists at the casting source level.

Future iterations may move casting rules into spell composition.

---

# Enemy Prototype

Added:

- Chaser enemy
- Tank enemy
- Swarm enemy

Enemy variety was introduced to evaluate spell effectiveness and combat pacing.

The enemy system is still considered prototype quality.

---

# Runtime Object System

Validated.

Runtime objects:

- Maintain gameplay state
- Own runtime lifetime
- Interact with the world
- Expose modification points

Current runtime objects:

- ProjectileRuntimeObject
- ExplosionRuntimeObject
- AuraRuntimeObject
- BeamRuntimeObject

---

# Module System Evolution

The module architecture has been expanded.

Modules are no longer considered only as event listeners.

Modules can now also modify runtime objects when appropriate.

General principle:

Modules modify existing behavior capabilities.

They do not replace behaviors.

---

Examples:

ProjectileBehavior

+

Homing Module

=

Homing projectile

---

ProjectileBehavior

+

Chain Module

=

Projectile with chained targeting

---

ProjectileBehavior

+

Speed Modifier

=

Projectile with modified movement behavior

---

The same concept should remain applicable to other runtime objects where meaningful.

---

# Behavior Responsibility

Behaviors define the fundamental identity of a spell.

Current behaviors:

- ProjectileBehavior
- AuraBehavior
- BeamBehavior

Examples:

Projectile:

Creates moving runtime objects.

Aura:

Creates persistent area-based runtime objects.

Beam:

Creates directional sustained runtime objects.

---

Modules enhance behaviors.

They should not become replacements for behaviors.

---

# Module Categories

Implemented.

Modules currently have categorization support to improve reward quality and future build rules.

Current purpose:

- Improve reward generation
- Avoid meaningless choices
- Prepare future compatibility rules

Future systems may introduce:

- Tags
- Compatibility restrictions
- Exclusive module categories

---

# Runtime Object Modification

Validated.

A generic runtime object modification approach has been introduced.

The goal:

Avoid creating systems tied only to projectiles.

Instead:

Modules should modify runtime capabilities when those capabilities exist.

Example:

A movement modifier affects runtime objects with movement.

A targeting modifier affects runtime objects capable of targeting.

---

# Current Implemented Modules

## Damage / Effects

- FireModule
- ExplosionModule
- CastSpellOnDestroyModule

---

## Projectile Behavior Modifiers

- ForkModule
- SplitOnDestroyModule
- HomingModule
- ChainModule
- Projectile speed modification modules

---

## Stat Modifiers

- SizeModifierModule

---

# Current Architecture Status

The spell composition architecture remains validated.

Current flow:

SpellDefinition

↓

SpellConfiguration

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

↓

Run Progression

---

# Validated Design Principles

✓ Behaviors define spell existence

✓ Modules extend behaviors

✓ Runtime objects contain gameplay state

✓ Runtime objects can be modified generically

✓ ScriptableObjects contain configuration only

✓ Views represent runtime objects only

✓ Stats are composed from behaviors and modules

✓ Player configurations are separated from runtime execution

✓ Runtime spell instances are disposable

✓ Combat consumes spell-generated events

✓ Rewards modify configurations, not active spells

✓ Module combinations can create emergent gameplay

---

# Current Limitations

Current:

- Duplicate module rules are undefined
- Reward rarity does not exist
- Module compatibility rules do not exist
- Tags system does not exist
- Build restrictions do not exist
- Enemy system remains prototype
- Combat objectives are limited
- Enemy scaling does not match spell scaling
- Some builds still converge toward direct damage optimization

---

# Deferred Systems

Not part of the MVP validation milestone:

- Inventory
- Meta progression
- Shops
- Save system
- Status effects
- Procedural generation
- Dungeon structure
- Floor progression
- Full progression tree

These systems should be designed after the core spell identity and gameplay loop are expanded.

---

# Next Phase

## Post-MVP Game Development Planning

The next milestone will focus on transforming the validated prototype into a complete game direction.

Main topics:

- Core gameplay loop definition
- Player progression structure
- Build rules
- Module identity
- Tags and compatibility systems
- Enemy design
- Boss encounters
- Presentation and visual identity
- Story hooks and world building

The objective is no longer validating the architecture.

The objective is defining the actual game built on top of it.

# Post MVP Transition

The spell architecture is validated.

The next milestone introduces:
- run lifecycle
- room progression
- research choices
- spell editing experiments
- minimal knowledge persistence

Existing spell ownership remains unchanged:
SpellConfiguration is player-owned.
SpellInstance remains temporary.