# Architecture

Current cross-system architecture. Code and tests are authoritative if this document becomes stale.

## Assemblies

`UnboundArcana.Runtime`

* Gameplay and runtime-facing UI/tools.
* No `UnityEditor` dependency.

`UnboundArcana.Editor`

* Editor-only authoring tools and inspectors.
* Depends on Runtime.

`UnboundArcana.Tests.Editor`

* Edit Mode tests for scene-independent systems.

## Core Rules

* ScriptableObjects: authored configuration.
* Runtime state: runtime instances.
* Prefer existing ownership over new managers/services.
* Events communicate across systems where direct ownership is inappropriate.
* Event subscriptions require explicit lifecycle ownership.
* Views represent gameplay state; they do not own gameplay rules.

## Global Runtime

`GameRuntimeManager` provides global gameplay services/context.

Global communication uses `GameEventBus`.

Use direct global access sparingly when explicit dependencies allow easier ownership/testing.

## Entities

`Entity` is the gameplay entity root.

Entity-specific state and communication remain local to the entity where possible.

Relevant systems include:

* stats
* health/damage
* statuses
* movement
* sensing/AI
* casting

Entity-local events use `EntityEventBus`.

Combat systems should not depend on concrete spell implementations.

## Spell Ownership

```text
SpellDefinition
-> SpellConfiguration
-> SpellSlot
-> SpellFactory
-> SpellInstance
-> SpellBehavior
-> SpellRuntimeObject
-> SpellRuntimeView
```

`SpellDefinition`

* authored initial spell configuration

`SpellConfiguration`

* mutable player-owned composition
* behavior + modules + configured spell values
* affects future casts only

`SpellSlot`

* owns persistent casting state for one equipped spell
* includes cooldown state

`SpellInstance`

* one temporary execution
* never persistent player-owned spell state

See `SpellSystem.md` for domain rules.

## Spell Runtime

`SpellRuntimeManager`

* tracks active `SpellInstance`s
* handles runtime teardown

`SpellInstance`

* owns behavior
* owns modules
* owns runtime objects
* owns `SpellEventBus`
* owns runtime stats/context

Behaviors define spell identity.

Modules extend behaviors through events, stat contributions and runtime capabilities.

Runtime objects own active gameplay state.

Views are Unity representations of runtime objects.

## Casting

Casting source selects a `SpellSlot`, creates an execution and supplies `CastContext`:

* Owner
* Position
* Direction

Simplified lifecycle:

```text
request cast
-> optional cast time
-> create/execute SpellInstance
-> behavior creates runtime objects
-> runtime objects finish
-> SpellInstance finishes
-> runtime teardown
```

Casting state such as cooldown belongs outside `SpellInstance`.

Continuous behaviors may retain external control through explicit capabilities such as `IContinuousSpellBehavior`.

## Stats

`StatCollection` is the common modifier model.

Values combine:

* bases
* flat modifiers
* percentage modifiers
* multipliers

Modifiers have sources and can be removed by source.

See `StatsSystem.md`.

## Events and Combat

Three event scopes:

* `GameEventBus`: global gameplay
* `EntityEventBus`: one entity
* `SpellEventBus`: one spell execution

Typical spell damage flow:

```text
RuntimeObject
-> HitEvent
-> Behavior / Modules
-> DamageEvent
-> DamageSystem
-> IDamageable
-> Entity events
```

Cross-system gameplay should depend on events/contracts rather than concrete spell types.

## Statuses

Statuses belong to entities and are separate from spell behaviors.

Spells may apply or interact with statuses.

See `StatusSystem.md`.

## Expedition

`ExpeditionRuntimeController` owns high-level expedition flow:

* expedition start/end
* floor progression
* room generation/transition
* expedition state
* room/research event coordination

Detailed player operations are delegated to `ExpeditionPlayerCoordinator`:

* spawn
* room placement
* input state
* camera coordination
* reveal/transition presentation

Research reward spawning is delegated to `ResearchRewardSpawner`.

These collaborators are regular runtime C# objects, not scene components.

## Floor and Rooms

Hierarchy:

```text
Expedition
-> Floor
-> Room
-> Room Sections
```

Rooms are assembled from authored content.

`RoomSection` provides runtime spatial data:

* connectors
* footprint
* markers
* grid
* props

Editor authoring operations remain in Editor code.

Runtime room flow handles gameplay state such as encounters, objectives, completion and transition to the next room.

## Input

Unity Input System is used.

Generated controls:

`Assets/UnboundArcana/Input/UnboundArcanaControls.inputactions`

Generated C# wrapper remains inside Runtime.

## Stable Principles

* Configuration and runtime state are separate.
* Spell configuration and spell execution are separate.
* Behaviors define spell identity.
* Modules extend; they do not replace behaviors.
* Runtime objects own spell gameplay state.
* Views do not own gameplay rules.
* Combat is decoupled from spell implementations.
* Editor and Runtime dependencies remain separated.
* Prefer capability-based extensions over concrete runtime-type coupling.
* Avoid abstraction without a demonstrated use case.
