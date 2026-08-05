# Design Documentation Index

Use this file to locate project context.

Do not read all documentation by default. Read only documents relevant to the current task.

Code and tests are authoritative for current implementation.

## GameVision.md

Player experience, core pillars and long-term game direction.

Read for:

* gameplay design
* Research
* combat direction
* Tower experience
* progression philosophy
* rewards
* enemies/objectives/Guardians
* evaluating whether a feature supports the game

Usually skip for implementation-only tasks with no gameplay-design impact.

## Research.md

Player-facing Research, Knowledge and run-progression design.

Read for:

* Minor and Major Rewards
* Knowledge
* Laboratories
* spell-evolution pacing
* Research UI
* Research MVP scope
* Research validation
* future Research expansion boundaries

For Research implementation tasks, also inspect current code.

Read `Architecture.md` or `SpellSystem.md` only when the change affects their ownership or domain rules.


## CurrentState.md

Current milestone, implemented foundations, weaknesses and active priorities.

Read for:

* project status
* prioritization
* planning
* deciding what to build next
* evaluating current maturity against the vision

Skip when the task already defines a narrow implementation goal.

## MacroImplementationPlan.md

Authoritative macro implementation checklist for the current releasable demo milestone.

Read for:

* macro task order and dependencies
* demo scope and explicit exclusions
* per-task acceptance criteria and validation
* Codex session workflow

Update task status and notes after each implementation session.

## Architecture.md

Current cross-system ownership and technical boundaries.

Read for:

* cross-system changes
* ownership/lifecycle questions
* new systems
* events
* runtime/editor boundaries
* expedition/room architecture
* changes affecting multiple gameplay domains

For local changes, inspect code first and read this only if ownership is unclear.

## SpellSystem.md

Spell composition and execution invariants.

Read for changes involving:

* SpellDefinition
* SpellConfiguration
* SpellSlot
* SpellInstance
* Behaviors
* Modules
* Runtime Objects
* spell casting lifecycle
* spell runtime events/capabilities

Do not read for unrelated gameplay systems.

## StatsSystem.md

Shared runtime stat/modifier model.

Read when changing:

* stats
* modifiers
* modifier sources
* temporary stat effects

## StatusSystem.md

Entity status ownership, effects and lifecycle.

Read when changing:

* statuses
* status interactions
* status-driven stat/event effects
* spell/status boundaries

## EditorTools.md

Editor/runtime authoring boundaries and existing editor tooling.

Read for:

* custom inspectors/windows
* asset authoring tools
* room-section authoring
* Tilemap/grid authoring workflows
* Editor vs Runtime placement

## Decisions.md

Intentional architectural and design decisions likely to be reconsidered.

Read when:

* proposing a significant architectural/design change
* replacing an established pattern
* introducing a general framework
* reconsidering progression, synergy, composition or Tower direction

Do not read for routine implementation.

## ScopeAndRisks.md

Scope and complexity guardrails.

Read before:

* large new systems
* highly combinatorial mechanics
* procedural generation systems
* broad abstractions/frameworks
* significant scope expansion

Skip for normal feature implementation.

## LoreAndIdentity.md

Narrative identity, Tower fiction and magical themes.

Read for:

* narrative/content design
* thematic naming
* worldbuilding
* presentation requiring fiction context

Skip for technical implementation unless theme affects the task.

## Archive/

Historical project documents.

Do not use as current project context.

Read only when explicitly investigating project history or past reasoning.

## Context Selection

Prefer the smallest useful context.

Typical routing:

```text
Local bug
-> code

Local feature
-> code
-> relevant domain doc if needed

Spell feature
-> code + SpellSystem

Cross-system feature
-> code + Architecture
-> relevant domain doc if needed

Gameplay/design review
-> GameVision + CurrentState
-> relevant domain doc if needed

Research design
-> GameVision + CurrentState + Research
-> Decisions when reconsidering established boundaries

Research implementation
-> code + Research
-> Architecture or SpellSystem only when ownership requires them

Project planning
-> CurrentState + GameVision

Major system proposal
-> GameVision + CurrentState + Decisions + ScopeAndRisks
-> relevant domain doc
-> Architecture if technically relevant

Editor tooling
-> code + EditorTools
```

Do not load a document only because it exists.

Retrieve additional context only when it can materially affect the task.

