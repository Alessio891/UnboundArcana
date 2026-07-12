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

# Modules

Implemented:

- FireModule
- ExplosionModule
- ForkModule
- SplitOnDestroyModule
- CastSpellOnDestroyModule
- SizeModifierModule

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

Views:

- Represent Unity objects only

---

# Projectile Runtime Handling

Projectile runtime objects currently support:

- Movement
- Lifetime
- Collision handling
- Runtime stat queries
- Hit event generation
- Hit history tracking

Hit history was added to prevent spawned projectiles from immediately repeating invalid interactions.

Example:

A split projectile should create additional gameplay opportunities rather than repeatedly damaging the same target through recursive collisions.

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

# Session 4 Progression Prototype

Implemented:

## Enemy Waves

EnemyWaveSpawner manages encounter progression.

Responsibilities:

- Spawn wave enemies
- Detect encounter completion
- Publish EncounterCompletedEvent
- Wait for reward selection
- Begin the next wave

---

## Reward Controller

RewardController manages temporary run progression.

Responsibilities:

- Listen for EncounterCompletedEvent
- Generate temporary reward offers
- Apply selected modules to the player's SpellConfiguration
- Publish RewardSelectedEvent

RewardController does not interact with SpellInstances directly.

---

## Reward Flow

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

Next Wave

---

# Combat Prototype Extensions

Added after playtest:

## Player Combat

Implemented:

- Player health
- Damage reception
- Defeat state


## Enemy Combat

Implemented:

- Enemy health
- Contact damage
- Enemy movement pressure
- Enemy scaling


## Enemy Archetypes

Prototype enemies:

- Chaser
- Tank
- Swarm

Enemy variety is currently intended for testing combat pacing and spell effectiveness.

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

✓ Player spell configuration is separated from runtime execution

✓ Runtime spell instances are disposable

✓ Combat systems consume game events instead of depending on spells

✓ Spell progression modifies SpellConfiguration rather than active runtime spells

✓ Reward progression integrates without modifying the spell runtime architecture

✓ Runtime object spawning can preserve explicit composition rules

---

# Current Limitations

Current:

- Duplicate module behavior is not yet defined
- Reward generation is random only
- Reward rarity does not exist
- Module synergies are limited
- Enemy system is still a prototype
- Combat objectives are limited
- Enemy scaling does not currently match spell scaling
- Spell builds often converge toward direct damage stacking

Future:

- Explicit duplicate module rules
- Reward weighting
- Reward categories
- Advanced module interactions
- Better enemy encounter design
- More meaningful build decisions