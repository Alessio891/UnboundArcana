# Decision: Spell Runtime Lifecycle Ownership

Date:

2026-07-12

## Context

Originally SpellInstances were treated as reusable runtime versions of a spell.

This created a problem:

Player-owned spell configuration and runtime gameplay state were mixed together.

A spell configuration can change over time.

A runtime spell must represent one execution.

## Decision

SpellConfigurations own spell composition.

SpellInstances represent temporary execution.

Flow:

SpellConfiguration

↓

SpellFactory

↓

SpellInstance

↓

Runtime Objects


SpellInstances are created per cast.

## Consequences

Positive:

- Runtime state cannot leak between casts.
- Spell modifications affect future casts only.
- Multiple independent casts are possible.
- Player ownership is separated from gameplay execution.

Negative:

- More runtime allocation occurs.
- Persistent spell behaviors require explicit lifecycle handling.