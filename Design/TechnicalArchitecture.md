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

# Spell Lifecycle Control

Spell instances are controlled by external casting sources.

Casting sources may include:

- Player input
- Enemy AI
- Environment systems
- Network-controlled actors

The spell system does not know the origin of these commands.

Lifecycle commands:

## Cast

Creates and starts the spell.

Example:

CastContext

↓

SpellInstance.Cast()

↓

SpellBehavior.Cast()


## UpdateCast

Updates active casting parameters.

Used for spells that require continuous external control.

Examples:

- Beam aiming
- Guided projectiles
- Charging spells


## End

Signals the end of external control.

Used for:

- Channelled spells
- Guided spells transitioning to independent behavior
- Temporary maintained effects


Behaviors decide how these commands affect their runtime objects.

Behaviors that do not require these lifecycle stages may ignore them.

---

# Behaviors

Behaviors define how a spell exists.

Responsibilities:

- spawning
- movement
- lifetime
- runtime object creation
- interpreting spell lifecycle commands

Behaviors never know modules exist.

Examples:

- Projectile
- Beam
- Aura
- Trap
- Minion

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

Runtime objects may receive state changes from their owning behavior.