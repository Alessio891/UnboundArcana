# Status System

Statuses are entity-owned gameplay conditions.

They support temporary or persistent effects and interactions, but are not a generic ability framework.

## Model

`StatusDefinition`

* authored ScriptableObject configuration

`StatusInstance`

* runtime state attached to an entity

Runtime state may include:

* duration
* stacks
* source/owner context
* effect-specific state

Exact fields depend on the implementation.

## Ownership

Statuses belong to entities, not spells.

Spells may:

* apply statuses
* react to statuses
* consume/modify statuses

An `Aura` is a spell Behavior, not an entity status.

## Effects

Statuses may:

* modify stats
* react to entity events
* deal damage
* affect movement/behavior
* expose state for spell/environment interactions
* drive presentation

Temporary stat changes should use source-based modifiers so teardown removes them safely.

## Design Rules

Statuses should create meaningful gameplay state or interactions.

Prefer:

* readable effects
* deterministic interaction rules
* reusable entity-level state

Avoid:

* using Status as a generic container for unrelated mechanics
* hidden interactions that require memorizing recipes
* duplicating systems already owned by spell runtime or entity components

Cross-spell interactions should emerge from explicit state and capabilities rather than hardcoded module-to-module dependencies.

## Lifecycle

Status runtime state must have explicit ownership and teardown.

Expiration/removal must clean up:

* stat modifiers
* event subscriptions
* presentation/resources owned by the status

Permanent effects are allowed only when Status is the appropriate semantic owner; permanence alone is not a reason to use the status system.
