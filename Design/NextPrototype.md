# ForbiddenKnowledgePrototype.md

# Unbound Arcana - ForbiddenKnowledgePrototype

## Purpose

This milestone defines the transition from a validated spell composition prototype into the actual game structure.

The technical foundation is considered validated.

The current goal is not proving the spell system works.

The goal is defining a realistic and engaging game around it.

---

# Validation Completed

The MVP validated:

## Spell Composition

The player can create spells through combinations of:

* Behaviors
* Modules / Principles
* Runtime interactions

---

## Gameplay Integration

Validated systems:

* Combat
* Enemy interaction
* Runtime execution
* Rewards
* Progression

---

## Emergent Behavior

Validated:

* Spell evolution
* Interesting combinations
* Player experimentation

---

# Current Milestone Goals

Define:

* Research expedition structure
* Combat philosophy
* Spell evolution rules
* Progression model
* Tower structure
* Instability system

---

# Current Design Hypotheses

## Spell Management

Baseline:

* 2 active spells
* 1 passive spell

Spells represent magical experiments.

The player maintains a small number of complex creations rather than many simple ones.

---

## Spell Modification

Preferred loop:

```
Initial Configuration
        |
Combat Trial
        |
Discovery
        |
Experimentation
        |
Advanced Trial
```

Major spell changes happen during research moments.

---

## Spell Replacement

A spell can evolve.

Changing behavior may remove incompatible principles.

The player rewrites experiments instead of simply replacing equipment.

---

## Deterministic Results

The player should understand magic.

Examples:

Projectile + Explosion always produces explosive projectile behavior.

Complexity comes from combinations, not hidden randomness.

---

## Instability

Instability is a risk/reward system.

It represents pushing magic beyond safe limits.

Potential direction:

* Complexity increases instability.
* Instability produces weighted events.
* Events create opportunities and risks.

Avoid making instability a simple failure chance.

---

# Implementation Awareness

The following ideas are intentionally constrained:

## Procedural Research

Prefer authored research domains with procedural combinations.

Avoid generating completely new research systems dynamically.

---

## Spell Interactions

Prefer explicit interaction points.

Avoid unrestricted interactions between all possible principles.

---

## Runtime Mutation

Prefer modifying future spell casts.

Avoid requiring all existing runtime objects to transform dynamically.

---

# Open Questions

Still requiring design:

* Combat encounter structure
* Enemy roles
* Boss philosophy
* Exact reward system
* Research branch implementation
* Instability details
* Resource systems
