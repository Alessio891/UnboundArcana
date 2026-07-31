# Scope and Risks

Unbound Arcana is a systemic game developed with limited production capacity.

Protect the core fantasy while controlling combinatorial and content cost.

## Protect

Core priorities:

* player-created spell composition
* deterministic magic
* spell evolution during runs
* knowledge/possibility progression
* responsive combat that tests builds

Do not simplify these away to make peripheral systems easier.

## Preferred Approach

Prefer:

* authored mechanics
* explicit runtime capabilities
* reusable systems with demonstrated use cases
* authored content assembled procedurally
* small experiments before large systems

Avoid:

* infrastructure for hypothetical requirements
* fully procedural generation of mechanics/content
* universal interaction frameworks
* systems whose complexity grows faster than their gameplay value

## High-Risk Areas

### Combinatorial Spell Interactions

Every mechanic interacting with every other mechanic becomes difficult to design, test and balance.

Use explicit capabilities and state boundaries.

Do not promise universal compatibility.

### Procedural Content

Prefer procedural assembly of authored rooms, encounters and rules.

Procedural generation of entirely new mechanics/content requires strong justification.

### Instability

Instability can undermine player trust if it makes spell behavior arbitrary.

Keep deterministic core behavior.

Instability should add controlled risk/opportunity, not random failure.

### Progression

Avoid permanent numerical escalation becoming the main progression model.

Prefer unlocking behaviors, modules, interactions and possibilities.

### Architecture

Systemic gameplay does not justify generalized frameworks by itself.

Abstract after multiple concrete use cases establish a stable common pattern.

## Decision Test

Before adding a system ask:

1. Does it improve the core spell/combat experience?
2. Is there a concrete current use case?
3. Can an existing system solve it?
4. Can we test the idea with a smaller implementation?
5. Does maintenance/content cost scale reasonably?

If several answers are unfavorable, defer the system.
