# Spell System

Domain rules for spell composition and execution.

For cross-system ownership see `Architecture.md`.

## Model

```text
SpellDefinition
-> SpellConfiguration
-> SpellSlot
-> SpellFactory
-> SpellInstance
-> Behavior
-> Runtime Objects
```

## SpellDefinition

Authored ScriptableObject defining an initial spell.

Configuration only. Never runtime state.

## SpellConfiguration

Mutable player-owned spell build.

Contains the composition used for future casts.

Changing it must not mutate existing `SpellInstance`s.

## SpellSlot

Owns one equipped `SpellConfiguration` and persistent state associated with casting that slot.

Cooldown runtime state belongs here.

A slot persists across individual casts; a `SpellInstance` does not.

## SpellInstance

One spell execution.

Created per cast.

Owns:

* behavior instance
* module instances
* runtime objects
* runtime stats
* `SpellEventBus`
* runtime context

Destroyed after its execution finishes.

Never use `SpellInstance` to store persistent loadout/cooldown state.

## Cast Context

Execution receives contextual information through `CastContext`:

* owner
* origin/position
* direction

The spell system should not require the casting source to be the player.

Possible sources include player, AI and gameplay systems.

## Casting Lifecycle

Conceptually:

```text
cast request
-> casting conditions
-> optional cast time
-> SpellInstance execution
-> runtime objects
-> completion
-> teardown
```

Cooldown and cast time must remain distinct concepts.

Holding input may request repeated casts, but each cast must still respect slot/casting rules.

Continuous spells may expose explicit lifecycle control such as update/end capabilities.

Do not force all behaviors into continuous semantics.

## Behavior

A Behavior defines the fundamental way a spell exists.

Current examples:

* Projectile
* Aura
* Beam

Behavior responsibilities:

* interpret cast lifecycle
* create appropriate runtime objects
* define fundamental spell identity

Behaviors do not know which modules are installed.

New fundamental spell existence usually implies a Behavior; modification of existing behavior usually implies a Module.

## Modules

Modules extend spell behavior without replacing its identity.

They may use:

* spell events
* stat contributions
* runtime-object capabilities

Prefer capability-based interaction.

Example:

A targeting module should depend on targeting capability rather than `ProjectileRuntimeObject` when the concept applies beyond projectiles.

Concrete-type coupling is acceptable when the mechanic is genuinely type-specific.

Avoid abstraction for hypothetical future combinations.

## Runtime Objects

Runtime objects own active spell gameplay state.

Possible responsibilities:

* position/direction
* movement
* lifetime
* targeting
* hit history
* collision interaction
* runtime modifiers

Runtime objects expose capabilities needed by reusable modules.

Destruction must be safe against repeated requests and emit destruction semantics only once.

## Views

`SpellRuntimeView` represents a runtime object using Unity objects.

Rules:

* runtime gameplay state remains in the runtime object
* runtime objects must not require concrete view classes
* views may own presentation lifecycle/animation

Presentation differences should not move gameplay ownership into Views.

## Stats

Behavior and module definitions contribute stats when an instance is created.

Runtime stats belong to the `SpellInstance`.

A configuration change affects subsequent instances.

Use the common stat/modifier model described in `StatsSystem.md`.

## Events

`SpellEventBus` is local to one `SpellInstance`.

Modules communicate through events/capabilities, not direct knowledge of other installed modules.

Runtime events may lead to global gameplay events such as damage.

Keep spell-internal and game-global event scopes distinct.

## Spawned Spell Content

Spawned runtime objects or secondary spell executions must preserve explicit composition semantics.

Do not implicitly share mutable runtime state between casts.

When one spell causes another execution, ownership/lifecycle must remain independent unless the mechanic explicitly requires otherwise.

## Composition Rules

Prefer:

* deterministic combinations
* emergent interaction through existing systems
* explicit capabilities
* behavior-changing modules

Avoid:

* hardcoded predefined synergies
* module-to-module dependencies
* universal interaction abstractions without real use cases
* runtime mutation of unrelated active casts

Compatibility/category restrictions should solve demonstrated conflicts rather than anticipate every future module.

## Invariants

Preserve these unless intentionally redesigning the spell architecture:

1. `SpellConfiguration` is persistent composition.
2. `SpellSlot` owns per-slot persistent casting state.
3. `SpellInstance` is one execution.
4. Behavior defines fundamental identity.
5. Modules extend Behavior/runtime capabilities.
6. Runtime Objects own active gameplay state.
7. Views own presentation, not gameplay rules.
8. Existing casts are independent from later configuration changes.
9. Module lifecycle and event subscriptions end with their owning instance.
10. Spell mechanics must not assume player-only casting.
