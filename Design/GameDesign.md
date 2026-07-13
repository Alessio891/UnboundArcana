# Unbound Arcana - Game Design Document

## High Concept

Unbound Arcana is a 2D top-down ARPG roguelike focused on creating unique, player-driven spells.

The player is a customizable mage who explores dangerous magical environments, discovering new magical components and combining them into increasingly unusual spell constructions.

The core fantasy is:

"Create a spell nobody else could have imagined."

The game is not about collecting predefined spells.

It is about constructing a personal magical system through combinations.

---

# Gameplay Pillars

## 1. Spell Creativity

The primary gameplay mechanic is building spells from:

* Behaviors
* Modules
* Stats
* Runtime interactions
* Events

Players should be able to create:

* extremely powerful builds
* strange experimental builds
* specialized builds
* inefficient but interesting builds

The player's identity comes from the magic they create.

---

## 2. Fast ARPG Combat

Combat should feel:

* fast
* responsive
* skill-based
* chaotic

Reference points:

* Hades
* Noita
* Risk of Rain
* Magicka
* Tiny Rogues

Combat exists to showcase spell construction.

The goal is not simply defeating enemies.

The goal is seeing how the player's spell behaves in a real environment.

---

## 3. Roguelike Progression

Each run is temporary.

During a run:

* discover modules
* modify spells
* adapt builds
* react to encounters

Between runs:

* unlock possibilities
* expand the available spell pool
* unlock additional systems

---

# Gameplay Loop

Current prototype loop:

Encounter

↓

Fight enemies

↓

Complete encounter

↓

Choose module reward

↓

Modify spell configuration

↓

Continue

---

Future game loop:

Preparation

↓

Enter magical environment

↓

Explore encounters

↓

Acquire spell components

↓

Modify spell builds

↓

Face stronger enemies

↓

Boss encounter

↓

New area / progression

↓

Return or continue

---

# Spell Philosophy

A spell is not a predefined ability.

A spell is a composition of:

* Behavior
* Modules
* Stats
* Events
* Runtime interactions

Examples:

---

Projectile + Fire + Explosion

Creates:

An explosive fire projectile.

---

Projectile + Homing + Split

Creates:

A projectile that seeks enemies and fragments after impact.

---

Projectile + Chain + Speed Modification

Creates:

A fast-moving projectile that jumps between targets.

---

The system should encourage players to discover combinations rather than follow predefined recipes.

---

# Spell Structure

## Behaviors

Behaviors define what type of spell exists.

Examples:

Projectile:

Creates moving spell objects.

Aura:

Creates persistent spell objects around an origin.

Beam:

Creates directional sustained spell objects.

---

## Modules

Modules modify existing spell behavior.

Examples:

Projectile modules:

* Homing
* Chain
* Split
* Movement changes
* On-hit effects

General modules:

* Size modification
* Duration modification
* Damage changes
* Runtime interactions

---

Modules should expand possibilities without replacing behaviors.

---

# Design Philosophy

The game should prioritize:

1. Spell system
2. Combat feel
3. Player experimentation
4. Build decisions
5. Content expansion

The spell system is the main feature.

Everything else exists to support it.

---

# Current Validation

The spell composition system has validated:

* Behaviors define spell existence.
* Modules extend behaviors through events.
* Runtime objects contain gameplay state.
* Runtime objects can be modified.
* Spawned objects can preserve composition rules.
* Different runtime events can drive different spell interactions.
* Player spell progression works inside a gameplay loop.

---

# Current Design Questions

The MVP validated the core mechanic.

The next phase must answer:

## Build Identity

How much freedom should players have?

Possible future systems:

* Module categories
* Tags
* Compatibility rules
* Exclusive choices
* Limited slots

---

## Progression

How does a build evolve during a run?

Questions:

* How many choices should players receive?
* Should modules have rarity?
* Should choices be random or influenced?
* Should some choices lock others?

---

## Combat Identity

How does the game create interesting decisions?

Possible directions:

* Enemy resistances
* Enemy behaviors
* Environmental interactions
* Encounter objectives
* Boss mechanics

---

# Current Development Status

## Completed MVP Validation

Validated:

- Spell composition architecture
- Runtime spell lifecycle
- Combat integration
- Reward progression
- Emergent module interactions

---

## Next Development Phase

Focus:

Move from prototype validation into game definition.

Areas:

- Core loop
- Player progression
- World structure
- Presentation
- Narrative hooks
- Enemy design
- Build rules

---

# Deferred Systems

Not yet required:

- Inventory
- Meta progression
- Shops
- Save system
- Status effects
- Procedural generation
- Dungeon structure
- Floor progression

These systems should be introduced only when they support a clearly defined game loop.

---

# Long Term Vision

The final game should allow players to create spells that feel personal.

Two players should be able to start with the same basic components and end with completely different magical identities.

The system should reward experimentation.

The goal is not discovering the strongest predefined spell.

The goal is creating a spell that belongs to the player.