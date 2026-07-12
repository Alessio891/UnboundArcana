# First Playable Prototype Roadmap

## Purpose

This roadmap defines the minimum development path from the current validated spell architecture to a first playable prototype.

The objective is not to build a complete game.

The objective is to validate two core questions:

1. Does the spell composition architecture remain robust when integrated into gameplay?
2. Is creating and modifying spells an engaging gameplay mechanic?

Each session represents one focused development milestone.

At the end of each session:

- Review completed work.
- Validate the intended gameplay or architectural goal.
- Update this document.
- Move to the next session only when the previous validation goal is satisfied.

---

# Current State

## Architecture Status

The spell composition architecture is validated.

Current flow:

SpellDefinition

↓

SpellConfiguration

↓

SpellFactory

↓

SpellInstance

↓

SpellBehavior

↓

SpellRuntimeObjects

↓

Runtime Events

↓

SpellModules

↓

Gameplay Systems

↓

Run Progression

---

# Session Status

Completed:

- Session 1 - Player Spell Configuration
- Session 2 - Combat Foundation
- Session 3 - Gameplay Variety
- Session 4 - Progression Loop

Next:

- Session 5 - First Playtest

---

# Session 1 - Player Spell Configuration

## Status

Completed.

## Validation Question

Can the player own and modify spell compositions independently from runtime spells?

Answer:

Yes.

---

# Session 2 - Combat Foundation

## Status

Completed.

## Validation Question

Can the existing spell architecture operate inside a real gameplay loop?

Answer:

Yes.

---

# Session 3 - Gameplay Variety

## Status

Completed.

## Validation Question

Do spell composition choices create meaningful gameplay differences?

Answer:

Yes.

---

# Session 4 - Progression Loop

## Status

Completed.

## Goal

Validate whether improving spells during gameplay creates meaningful decisions.

## Implemented

- Enemy wave encounters
- Reward phase between waves
- RewardController
- Random module reward offers
- SpellConfiguration modification during a run

## Validation Results

Validated:

- Spell improvements naturally affect future casts.
- Progression integrates without changing runtime spell execution.
- Different reward choices lead to different spell identities.
- The ownership model remains valid.

## Validation Question

Does acquiring and modifying spell components during gameplay create meaningful decisions?

Answer:

Yes.

---

# Session 5 - First Playtest

## Status

Next.

## Goal

Evaluate the complete combat and progression loop as a playable prototype.

Focus areas:

- Combat pacing
- Enemy variety
- Reward cadence
- Module balance
- Player decision quality
- Combat feedback

Avoid introducing large new systems until the existing loop has been thoroughly evaluated.

---

# Session 6 - Architecture and Gameplay Review

## Goal

Review all prototype findings before expanding the project.

Potential discussion topics:

- Duplicate module handling
- Reward weighting
- Progression depth
- Long-term spell growth
- Architecture adjustments informed by playtesting