# Architecture Snapshot

Last Updated:

2026-07-30

Unity Version:

Unity 6

---

# Current Architecture

The project is built around four main runtime areas:

- Spell composition and execution
- Entity, combat, and status systems
- Expedition, floor, and room progression
- Research and run progression

The UI is currently prototype quality and should not be treated as a stable architectural boundary.

---

# Assembly Boundaries

## UnboundArcana.Runtime

Contains gameplay, UI, sandbox, map, player, spell, entity, room, and expedition code.

Explicit dependencies:

- Unity Input System
- Unity UI
- TextMesh Pro
- Pixelplacement iTween

Runtime code must not depend on `UnityEditor`.

## UnboundArcana.Editor

Contains custom inspectors and room authoring tools.

This assembly:

- Compiles for the Editor only
- References `UnboundArcana.Runtime`
- Owns all `UnityEditor` dependencies

## UnboundArcana.Tests.Editor

Contains Edit Mode tests for systems that can be tested without scenes or prefabs.

Current coverage:

- `GameEventBus`
- `StatCollection`

## Pixelplacement.iTween

iTween has its own assembly so the runtime assembly can reference it explicitly.

---

# Spell Ownership

`SpellDefinition` is authored as a ScriptableObject.

`SpellConfiguration` is a mutable player-owned spell build containing:

- A `SpellBehaviorDefinition`
- A list of `SpellModuleDefinition`
- The configured cooldown value

`SpellSlot` owns casting state such as the cooldown timer.

`SpellInstance` represents one temporary execution and is never the persistent player-owned spell.

Flow:

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

Changing a `SpellConfiguration` affects future casts only.

---

# Spell Casting Lifecycle

The casting source creates a `SpellInstance` from the selected slot and supplies a `CastContext`.

The context contains:

- Owner
- Position
- Direction

Lifecycle:

```text
Create
    -> Initialize behavior and modules
    -> Optional cast time
    -> Cast
    -> Register with SpellRuntimeManager
    -> Runtime objects execute
    -> Runtime objects finish
    -> SpellFinishedEvent
    -> SpellRuntimeManager teardown
```

Spell behaviors are divided by capability:

- Autonomous behaviors release the caster after cast completion.
- Behaviors implementing `IContinuousSpellBehavior` retain external control until `End`.

`BeamBehavior` currently implements continuous control.

Cooldown state belongs to `SpellSlot`, not `SpellConfiguration` or `SpellInstance`.

---

# Spell Runtime Responsibilities

## SpellBehavior

Defines the fundamental identity and existence of a spell.

Current behaviors:

- Projectile
- Aura
- Beam

Behaviors:

- Create runtime objects
- Interpret cast lifecycle commands
- Do not know which modules are installed

## SpellModule

Extends a spell through:

- Spell events
- Runtime object modifiers
- Stat contributions

Modules are initialized and destroyed with their owning `SpellInstance`.

Event subscriptions must be removed during teardown.

## SpellRuntimeObject

Owns gameplay state such as:

- Position and direction
- Lifetime
- Hit history
- Runtime modifiers

Current runtime objects:

- `ProjectileRuntimeObject`
- `ExplosionRuntimeObject`
- `AuraRuntimeObject`
- `BeamRuntimeObject`

Destruction is idempotent and publishes `RuntimeObjectDestroyedEvent` once.

## SpellRuntimeView

`SpellRuntimeView` is the common base for Unity GameObject representations.

Current views:

- `ProjectileView`
- `ExplosionView`
- `AuraView`
- `BeamView`

The runtime object depends only on `SpellRuntimeView`, never on a concrete view type.

The default view lifecycle destroys the GameObject immediately. `ProjectileView` overrides this to play its ending animation before destruction.

---

# Stats

`StatCollection` and `SpellStatCollection` compose values from:

- Base values
- Flat modifiers
- Percentage modifiers
- Multipliers

Behavior definitions and module definitions contribute spell stats during factory creation.

Run modifiers are applied to the newly created `SpellInstance`.

Modifiers can be removed by their source.

---

# Events and Combat

The architecture uses three event scopes:

- `GameEventBus` for global gameplay communication
- `EntityEventBus` for events owned by one entity
- `SpellEventBus` for events owned by one spell execution

Subscriptions tied to MonoBehaviour availability use `OnEnable` and `OnDisable`.

Spell modules and behaviors remove their listeners during spell teardown.

Combat flow:

```text
Spell runtime object
    -> HitEvent
    -> Spell modules and behavior
    -> DamageEvent
    -> DamageSystem
    -> IDamageable
    -> Entity events
```

Combat systems consume events and do not depend directly on spell implementations.

---

# Expedition Runtime

`ExpeditionRuntimeController` owns the high-level expedition state and flow.

Responsibilities:

- Start the expedition
- Generate and advance floors
- Generate and transition rooms
- React to room and research events
- Coordinate expedition states
- Activate completed research

Detailed player operations are delegated to `ExpeditionPlayerCoordinator`.

`ExpeditionPlayerCoordinator` owns:

- Player spawning
- Movement between room start markers
- Input enable and disable
- Camera follow and snapping
- Player reveal presentation

Research reward pickup creation is delegated to `ResearchRewardSpawner`.

`ResearchRewardSpawner` owns:

- Reward selection from available research
- Spawn position selection
- Pickup creation and animation
- Cleanup of unselected pickups

These collaborators are regular C# classes created by `ExpeditionRuntimeController`. They are not MonoBehaviours and do not need to be added to scene GameObjects.

---

# Rooms and Authoring

`RoomSection` is a runtime component containing:

- Section identifier
- Connectors
- Footprint
- Markers
- Grid reference
- Prop renderer references
- Spatial queries

`RoomSection` contains no `UnityEditor` dependency.

Room authoring operations live in `RoomSectionEditor` and the existing Editor tooling.

Editor-only operations include:

- Section setup
- Tilemap creation
- Tilemap normalization
- Grid alignment
- Marker refresh
- Prop gathering
- Footprint gizmos

The runtime prop collection is exposed as read-only.

---

# Input

The project uses the Unity Input System.

`UnboundArcanaControls` is generated from:

`Assets/UnboundArcana/Input/UnboundArcanaControls.inputactions`

The generated wrapper is kept inside the Runtime assembly boundary:

`Assets/UnboundArcana/Input/UnboundArcanaControls.cs`

---

# Validated Principles

- ScriptableObjects contain authored configuration.
- Runtime state belongs to disposable runtime instances.
- Player spell builds and active spell executions are separate.
- Behaviors define spell identity.
- Modules extend behaviors without replacing them.
- Runtime objects own gameplay state.
- Views represent runtime objects without owning gameplay rules.
- Combat consumes spell-generated events.
- Runtime and Editor code are separated by assembly boundaries.
- Event subscriptions have explicit lifecycle ownership.
- Pure systems should receive Edit Mode test coverage.

---

# Current Limitations

- UI remains placeholder and is not architecturally stable.
- Duplicate module rules are not fully defined.
- Module compatibility and exclusion rules are limited.
- Reward selection is still mostly random.
- Reward rarity and weighting require further design.
- Enemy behaviors and combat objectives remain prototype quality.
- Some runtime systems still access global managers directly.
- Test coverage currently targets only the event bus and stat collection.
- Sandbox systems remain inside the Runtime assembly and may later deserve their own boundary.

---

# Recommended Next Architectural Work

- Expand tests around spell lifecycle, cooldowns, and module teardown.
- Add validation for spell definitions and prefab/view requirements.
- Decide explicit duplicate and compatibility rules for modules.
- Reduce direct singleton access where systems need isolated tests.
- Split prototype UI and sandbox code into separate assemblies when they become stable enough to justify the boundary.
