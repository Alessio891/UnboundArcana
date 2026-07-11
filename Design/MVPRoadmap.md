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

```
SpellDefinition

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
```

Implemented:

## Behaviors

- ProjectileBehavior
- AuraBehavior
- BeamBehavior

## Runtime Objects

- ProjectileRuntimeObject
- ExplosionRuntimeObject
- AuraRuntimeObject
- BeamRuntimeObject

## Modules

- FireModule
- ExplosionModule
- ForkModule
- SplitOnDestroyModule
- CastSpellOnDestroyModule
- SizeModifierModule

## Systems

- SpellEventBus
- GameEventBus
- Runtime Stat Aggregation
- Runtime Modifier System
- Spell Chaining
- Runtime Context Injection

---

# Prototype Philosophy

The prototype should prioritize validation over content.

Do not add systems because they are expected in the final game.

Add systems only when they answer a gameplay question.

Priority order:

1. Spell ownership and creation
2. Combat interaction
3. Spell variety
4. Progression choices
5. Extended playtesting

---

# Session 1 - Player Spell Configuration

## Goal

Create the missing bridge between authored spell data and player-owned spell builds.

Current:

```
SpellDefinition

↓

SpellInstance
```

Target:

```
SpellDefinition

↓

PlayerSpellConfiguration

↓

SpellFactory

↓

SpellInstance
```

---

## New Concepts

### PlayerSpellConfiguration

Represents a player's current spell build.

Example:

```
Projectile

+

Fire Module Level 3

+

Explosion Module Level 1
```

It contains:

- Selected behavior
- Selected modules
- Module progression values
- Future upgrade information

It does not contain:

- Runtime objects
- Views
- Active spell state
- Gameplay state

---

## Validation Question

Can the player own and modify spell compositions independently from runtime spells?

---

## Completion Criteria

The system can:

- Create a player-owned spell build.
- Modify the build.
- Generate a SpellInstance through SpellFactory.

---

# Session 2 - Combat Foundation

## Goal

Connect the spell system to a playable combat scenario.

---

## Required Systems

### Player

- Movement
- Aiming
- Casting

### Enemy

- Movement
- Health
- Damage reception
- Death

### World

- Arena
- Basic enemy spawning

---

## Required Gameplay Flow

```
Player

↓

Cast Spell

↓

Spell Runtime Objects

↓

Hit Event

↓

Damage Event

↓

Enemy Health

↓

Death
```

---

## Scope Restrictions

Do not add:

- Complex AI
- Status effects
- Resistances
- Procedural generation
- Bosses

---

## Validation Question

Can the existing spell architecture operate inside a real gameplay loop?

---

## Completion Criteria

A player can:

- Move.
- Cast a spell.
- Damage an enemy.
- Defeat an enemy.

---

# Session 3 - Gameplay Variety

## Goal

Introduce the minimum amount of content required to evaluate spell creativity.

The purpose is not content creation.

The purpose is creating meaningful choices.

---

# Minimum Behaviors

## Projectile

Tests:

- Aiming
- Collision
- Hit effects

---

## Aura

Tests:

- Persistent effects
- Area control

---

## Beam

Tests:

- Continuous casting
- External control

---

# Minimum Modules

## Effect Modules

### Fire

Purpose:

- Basic damage identity

### Ice

Purpose:

- Slow and control

### Lightning

Purpose:

- Fast impact and chaining potential

### Poison

Purpose:

- Damage over time identity

---

## Behavior Modules

### Split

Creates multiple projectiles.

### Pierce

Allows attacks to continue through targets.

### Bounce

Creates environmental interaction.

### Homing

Improves reliability against mobile targets.

---

## Trigger Modules

### Explosion

Creates secondary effects.

### Chain Spell

Creates additional spell compositions.

### Spawn Aura

Creates persistent effects from interactions.

---

# Minimum Enemy Types

Enemies should create different gameplay problems.

---

## Bruiser

Characteristics:

- High health
- Slow movement
- Direct threat

Tests:

- Sustained damage

---

## Swarm

Characteristics:

- Many weak enemies

Tests:

- Area damage
- Crowd control

---

## Runner

Characteristics:

- Fast movement
- Difficult to hit

Tests:

- Accuracy
- Tracking

---

## Turret

Characteristics:

- Stationary
- Creates positioning pressure

Tests:

- Movement decisions
- Area control

---

## Validation Question

Do different spell compositions create different combat strategies?

---

## Completion Criteria

Players can recognize situations where changing spell composition provides advantages.

---

# Session 4 - Progression Loop

## Goal

Validate whether improving spells is motivating.

---

## Required Systems

Reward selection.

Rewards modify:

```
PlayerSpellConfiguration

↓

SpellFactory

↓

New SpellInstance
```

---

## Reward Examples

Add module:

```
Fire Module
```

Upgrade module:

```
Fire Level 2 → Fire Level 3
```

Modify stats:

```
Increase Size
```

---

## Important Rules

Rewards do not directly modify active runtime objects.

Incorrect:

```
Reward

↓

Current Projectile

↓

Increase Damage
```

Correct:

```
Reward

↓

Player Spell Configuration

↓

Future SpellInstance
```

---

## Validation Question

Does improving a spell encourage experimentation and build decisions?

---

## Completion Criteria

A player can:

- Defeat enemies.
- Receive choices.
- Modify their spell.
- Continue fighting with the new build.

---

# Session 5 - First Playtest

## Goal

Create a stable repeatable prototype loop.

---

## Add Only Necessary Improvements

Possible additions:

- Multiple enemy waves.
- Basic balancing.
- Better visual feedback.
- Placeholder audio.
- Minimal UI.
- Quality-of-life improvements.

---

## Do Not Add

- Inventory
- Equipment
- Tower generation
- Meta progression
- Large content expansions

---

## Validation Question

Is the core loop enjoyable over repeated encounters?

---

## Completion Criteria

The prototype supports:

```
Create Spell

↓

Enter Combat

↓

Fight Enemies

↓

Receive Upgrade

↓

Improve Spell

↓

Continue Fighting
```

---

# Session 6 - Architecture and Gameplay Review

## Goal

Evaluate the prototype before expanding the game.

---

## Review Areas

### Spell Architecture

Questions:

- Are behaviors flexible enough?
- Are modules independent enough?
- Are new combinations easy to create?
- Are abstractions justified?

---

### Gameplay

Questions:

- Are spell choices meaningful?
- Are enemies creating different decisions?
- Are some modules always superior?
- Are some combinations boring?

---

### Future Direction

Decide:

- What systems should be expanded.
- What systems should be redesigned.
- What content should be added.
- What complexity should be avoided.

---

# Final Prototype Success Criteria

The prototype is successful when:

```
Create Spell

↓

Enter Combat

↓

Encounter Different Challenges

↓

Adapt Spell Build

↓

Feel Ownership Over The Creation

↓

Continue Playing
```

The goal is not proving that every system works.

The goal is proving that the central fantasy of Unbound Arcana works:

"Create a spell nobody else could have imagined."