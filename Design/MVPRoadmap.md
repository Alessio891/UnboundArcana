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

# MVP Status

The First Playable Prototype MVP is completed.

The prototype successfully validated:

- Spell composition architecture.
- Runtime spell generation.
- Gameplay integration.
- Module-based spell evolution.
- Player-driven spell modification.

The next phase should not focus on adding more prototype content.

The next phase should focus on transforming the validated foundation into a complete game structure.

---

# Session Status

Completed:

- Session 1 - Player Spell Configuration
- Session 2 - Combat Foundation
- Session 3 - Gameplay Variety
- Session 4 - Progression Loop
- Session 5 - First Playtest
- Session 6 - Architecture and Gameplay Review

Current:

MVP Complete

Next:

New development roadmap focused on game design, progression, presentation and content structure.

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

Completed.

## Goal

Evaluate the complete combat and progression loop as a playable prototype.

## Implemented

### Combat Improvements

Added:

- Player health system
- Enemy contact damage
- Enemy health scaling
- Enemy movement pressure
- Spell casting cooldown

The player and enemies now interact through the existing damage pipeline.

---

### Enemy Variety

Added prototype enemy archetypes:

- Chaser enemy
- Tank enemy
- Swarm enemy

Enemy scaling was introduced to allow encounters to increase in difficulty.

---

### Projectile Improvements

Added:

- Projectile hit history.

Purpose:

Prevent spawned projectiles from immediately repeating invalid interactions with the same target.

Example:

A split projectile should create new gameplay opportunities rather than simply multiplying damage on the same enemy.

---

## Playtest Results

### Combat Pacing

Validated:

- Base combat pacing is acceptable.
- Spell power growth currently outpaces enemy scaling.
- Encounters become significantly easier as modules accumulate.

Main cause:

Current reward choices mostly increase direct damage.

---

### Spell Evolution

Validated:

- Adding modules creates a feeling of spell evolution.
- Players can perceive a changing spell identity.
- Current choices are still limited and frequently converge toward damage stacking.

---

### Module Problems

Identified:

- Some modules create excessive scaling.
- Some interactions need additional rules.
- Module variety is currently insufficient to create distinct build paths.

---

### Casting

Identified:

Current casting works for:

- Projectile spells
- Beam spells

Missing:

- More varied casting interactions.
- Better support for sustained and alternative casting styles.

---

## Overall Validation

The core questions remain positive:

1. Does the spell composition architecture remain robust when integrated into gameplay?

Answer:

Yes.

The gameplay systems interact with spell events without requiring changes to spell ownership or runtime architecture.

2. Is creating and modifying spells an engaging gameplay mechanic?

Answer:

Partially validated.

The mechanic works and creates evolution, but the prototype does not yet provide enough variety and decision depth.

---

# Session 6 - Architecture and Gameplay Review

## Status

Completed.

## Goal

Review prototype findings before expanding the game scope.

Focus areas:

- Module balance
- Reward quality
- Build diversity
- Combat pacing
- Spell identity
- Runtime architecture flexibility

---

## Implemented

### Runtime Object Modifiers

Introduced runtime object extension through modifiers.

Validated that modules can extend runtime objects without requiring direct coupling.

Examples:

Projectile runtime extensions:

- Homing
- Chain
- Orbit
- Projectile acceleration

Explosion runtime extensions:

- Damage pulses

Aura runtime extensions:

- Periodic area damage

---

## Validation Results

### Module Variety

Validated:

Modules are no longer limited to numerical increases.

Modules can change:

- Movement
- Targeting
- Spawning
- Area interaction
- Runtime behavior

---

### Emergent Builds

Validated:

Interesting spell identities can emerge from player choices without explicit synergy rules.

Examples:

Missile builds:

- Projectile
- Homing
- Chain
- Acceleration

Swarm builds:

- Fork
- Split
- Chain

Area builds:

- Aura
- Size
- Aura damage

Explosion builds:

- Explosion
- Pulse
- Size

---

### Synergy System

Conclusion:

A traditional predefined synergy system is not required at this stage.

Player-driven composition creates emergent combinations naturally.

Future systems should focus on rules and constraints rather than predefined combinations.

---

### Module Restrictions

Identified future requirements:

- Module categories.
- Tags.
- Compatibility rules.
- Runtime object capabilities.

Example:

Movement modifying modules may eventually share a restriction category.

---

### Reward System

Validated:

Random module rewards create meaningful choices.

Identified future improvements:

- Better reward filtering.
- Module rarity.
- Module categories.
- More controlled progression.

---

# MVP Completion

The First Playable Prototype milestone is complete.

Validated questions:

## Does the spell composition architecture remain robust when integrated into gameplay?

Answer:

Yes.

The architecture supports:

- Multiple behaviors.
- Runtime objects.
- Event-driven modules.
- Gameplay integration.
- Progressive spell modification.

---

## Is creating and modifying spells an engaging gameplay mechanic?

Answer:

Yes, with further design work required.

The foundation is validated.

The next challenge is no longer architecture.

The next challenge is designing the actual game experience around spell creation.

---

# Next Development Phase

The next roadmap should focus on transforming the prototype into a complete game concept.

Main areas:

- Game loop.
- Player progression.
- Spell growth rules.
- Module identity.
- Tags and compatibility.
- Presentation.
- World and story hooks.
- Encounter structure.

Large systems should still be introduced only after the core experience is defined.

Possible future topics:

- Run structure.
- Meta progression.
- Dungeon/floor structure.
- Boss encounters.
- Narrative framework.
- Art direction.
- User interface.
- Audio feedback.