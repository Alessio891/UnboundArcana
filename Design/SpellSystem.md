# Architecture Snapshot

Last Updated:

2026-07-13

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

↓

Run Progression

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


RewardController

├── Temporary Reward Offer

└── Modifies Player SpellConfiguration

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

# Behaviors

Implemented:

- ProjectileBehavior
- AuraBehavior
- BeamBehavior

---

# Behavior Responsibility

Behaviors define the fundamental identity of a spell.

Examples:

ProjectileBehavior:

Creates moving projectile runtime objects.

AuraBehavior:

Creates persistent area-based runtime objects.

BeamBehavior:

Creates directional sustained runtime objects.

Modules enhance these behaviors.

They do not replace them.

---

# Modules

Implemented:

## Damage / Effects

- FireModule
- ExplosionModule
- CastSpellOnDestroyModule

---

## Projectile Modification

- ForkModule
- SplitOnDestroyModule
- HomingModule
- ChainModule
- Projectile movement modifiers

---

## Stat Modification

- SizeModifierModule
- Other stat modifier modules

---

# Module Architecture Evolution

Originally modules were mainly event listeners.

The system has expanded to support runtime object modification.

Current principle:

A module should modify capabilities exposed by runtime objects.

Examples:

Movement capability:

- Speed changes
- Acceleration
- Homing movement

Targeting capability:

- Homing
- Chain targeting

Lifetime capability:

- Duration changes

---

# Runtime Object Pattern

Runtime Object

↓

View

Runtime objects:

- Maintain gameplay state
- Query effective stats
- Control lifetime
- Handle world interaction
- Expose modification points

Views:

- Represent Unity objects only

---

# Runtime Object Modification

Validated approach:

Modules can modify runtime objects when the capability exists.

The architecture intentionally avoids projectile-only assumptions.

Examples:

Projectile:

- Movement modification
- Targeting modification
- Split behavior
- Chain behavior

Aura:

Future possible modifications:

- Radius
- Duration
- Interactions

Beam:

Future possible modifications:

- Width
- Duration
- Targeting

---

# Projectile Runtime Handling

Projectile runtime objects currently support:

- Movement
- Lifetime
- Collision handling
- Runtime stat queries
- Hit event generation
- Hit history tracking
- Runtime modifiers

Hit history prevents spawned projectiles from immediately repeating invalid interactions.

Example:

A split projectile should create additional gameplay opportunities rather than repeatedly damaging the same target.

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

↓

Enemy Killed Event

Combat systems remain separated from spell execution.

The spell system creates gameplay events.

Gameplay systems consume those events.

---

# Progression Prototype

Implemented:

## Enemy Waves

EnemyWaveSpawner manages encounter progression.

Responsibilities:

- Spawn wave enemies
- Detect encounter completion
- Publish EncounterCompletedEvent
- Wait for reward selection
- Begin next wave

---

## Reward Controller

RewardController manages temporary run progression.

Responsibilities:

- Listen for EncounterCompletedEvent
- Generate reward offers
- Filter available modules
- Apply selected modules to SpellConfiguration
- Publish RewardSelectedEvent

RewardController does not interact with SpellInstances directly.

---

# Reward Flow

Validated:

Wave Complete

↓

EncounterCompletedEvent

↓

RewardController

↓

Player chooses module

↓

SpellConfiguration updated

↓

RewardSelectedEvent

↓

Next Encounter

---

# Combat Prototype Extensions

Added:

## Player Combat

Implemented:

- Player health
- Damage reception
- Defeat state

---

## Enemy Combat

Implemented:

- Enemy health
- Contact damage
- Enemy movement pressure
- Enemy scaling

---

## Enemy Archetypes

Prototype enemies:

- Chaser
- Tank
- Swarm

Enemy variety currently exists to evaluate combat pacing.

---

# Proven Design Principles

✓ No individual spell classes

✓ Behaviors own spell existence

✓ Modules extend behaviors

✓ Modules do not communicate directly

✓ Runtime objects own gameplay state

✓ ScriptableObjects contain configuration only

✓ Views represent runtime objects only

✓ Stats are composed from behavior and module contributions

✓ Player spell configuration is separated from runtime execution

✓ Runtime spell instances are disposable

✓ Combat systems consume game events instead of depending on spells

✓ Spell progression modifies SpellConfiguration instead of active runtime spells

✓ Reward progression integrates without modifying spell execution

✓ Runtime objects can preserve explicit composition rules

✓ Runtime object modification is capability-oriented

---

# Current Limitations

Current:

- Duplicate module behavior is undefined
- Reward rarity does not exist
- Module compatibility rules do not exist
- Tags system does not exist
- Build restrictions do not exist
- Enemy system is still prototype
- Combat objectives are limited
- Enemy scaling does not match spell scaling
- Some builds converge toward direct damage optimization

---

# Deferred Systems

Not part of MVP validation:

- Inventory
- Meta progression
- Shops
- Save system
- Status effects
- Procedural generation
- Dungeon structure
- Floor progression

---

# MVP Status

The spell architecture and progression prototype milestone is complete.

Validated:

- Spell composition works.
- Runtime architecture scales with additional modules.
- Player-driven spell evolution creates emergent behavior.
- Combat integration does not compromise spell ownership.
- Reward progression modifies builds correctly.

---

# Next Phase

The next phase moves from architecture validation into game definition.

Focus:

- Define final gameplay loop
- Design progression systems
- Establish build rules
- Introduce tags when justified
- Expand enemy design
- Define world and presentation
- Create story hooks
- Improve player-facing clarity

The architecture is now considered a foundation for building the actual game.