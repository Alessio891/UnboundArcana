# Research and Run Progression

Current MVP design hypothesis.

This document defines the player-facing Research and run-progression loop.

Implementation ownership and technical details belong in architecture and code documentation once the design is implemented.

## Purpose

Run progression operates on two different scales:

* Minor Rewards evolve the expedition.
* Major Rewards evolve spell configurations.

Minor Rewards provide frequent progression without constantly changing spell identity.

Major Rewards occur less frequently and significantly modify a spell.

Knowledge is a temporary expedition resource used to increase player agency during Major Research.

## Core Loop

```text
Room
-> Combat / Objective
-> Room Complete
-> Minor Reward
-> Continue Expedition

Several Rooms
-> Laboratory
-> Spend Knowledge for additional research agency
-> Choose Major Reward
-> Modify SpellConfiguration
-> Enter Combat
-> Test evolved spell
```

Combat tests the current build.

Minor Rewards shape the conditions of the expedition.

Laboratories create major spell-evolution moments.

## Minor Rewards

Minor Rewards are offered after completing standard rooms.

MVP presentation:

```text
Choose one of three Run Modifiers.
```

The selected modifier activates immediately and lasts for the rest of the expedition.

Minor Rewards do not directly add, remove or replace spell modules.

They may affect:

* combat conditions
* survival
* Knowledge acquisition
* expedition risk and utility
* player or run-level stats

Prefer modifiers that affect decisions, incentives or conditions over unconditional numerical bonuses.

Example directions:

### Reactive Ward

The first time the player takes damage in a room, briefly increase movement speed.

### Dangerous Study

Elite encounters grant additional Knowledge, but elite enemies are stronger.

### Field Research

Optional objectives grant additional Knowledge.

These examples are not final content or balance targets.

Plain stat modifiers are acceptable during early prototyping but should not dominate the reward pool.

The MVP does not use:

* Minor Reward slots
* research queues
* research progress bars
* delayed reward activation
* Minor Reward currencies
* modifier replacement or removal

Selected Minor Rewards accumulate for the duration of the expedition.

A cap, replacement system or modifier evolution should only be introduced if playtests show that accumulated modifiers become excessive or unreadable.

## Knowledge

Knowledge is temporary and resets when the expedition ends.

It is earned through normal Tower play.

Initial sources may include:

* completing rooms
* completing objectives
* defeating enemies
* defeating elite enemies
* completing special encounters

Knowledge should not require repetitive research tasks or checklist-style combat behavior.

MVP principle:

```text
Knowledge increases Research agency.
Knowledge does not directly purchase baseline power.
```

The player always receives a Major Reward opportunity at a Laboratory.

Knowledge improves the available decision rather than unlocking access to that decision.

## Laboratories

Laboratories are dedicated strategic moments within the Tower.

They interrupt combat infrequently and provide significant spell-evolution choices.

MVP Laboratory flow:

1. Show the current spell configurations.
2. Offer three compatible Major Rewards.
3. Allow the player to spend Knowledge to reveal one additional option.
4. Let the player choose one Major Reward.
5. Apply it immediately to a valid spell configuration.
6. Continue the expedition and test the changed spell.

The initial Knowledge operation is:

### Analyze Further

Spend Knowledge to reveal one additional Major Reward.

Revealing an additional option is preferred over replacing existing options because it increases agency without discarding previous opportunities.

Possible future Knowledge operations include:

* reroll an option
* preserve an option for a future Laboratory
* focus the offer toward a domain or capability
* reveal information about future routes or Laboratories

These operations are not part of the MVP.

## Major Rewards

Major Rewards significantly modify `SpellConfiguration`.

They may include:

* Principles
* Modules
* meaningful slot changes
* other major configuration changes

Behavior replacement or the creation of additional spell experiments requires separate design validation and is not assumed by this MVP.

The initial implementation should primarily use existing compatible Principles and Modules.

Major Rewards should favor qualitative changes such as:

* additional projectiles
* explosions
* chaining
* homing
* piercing
* status application
* runtime-event interactions

Randomness determines which opportunities are offered.

Once selected, the resulting spell behavior remains deterministic and understandable.

## Floor Pacing Hypothesis

Initial test structure:

```text
Room
Room
Room
Laboratory

Room
Room
Room
Laboratory

Guardian
```

Starting parameters:

* one Minor Reward after each standard room
* approximately two Laboratories per floor
* approximately two or three combat rooms between Laboratories

These are prototype parameters, not permanent design decisions.

Laboratories may initially appear at fixed progression points.

Route-based access to Laboratories should only be introduced after baseline pacing has been validated.

## System Relationships

```text
Combat
-> earns Knowledge and exposes build properties

Minor Rewards
-> alter expedition conditions and incentives

Knowledge
-> increases agency at Laboratories

Laboratories
-> provide Major Rewards

Major Rewards
-> evolve spells

Evolved spells
-> change subsequent Combat
```

Avoid explicit hardcoded Minor and Major synergy recipes.

Interactions should emerge from existing stats, statuses, spell modules and gameplay rules.

## MVP Content Scope

Initial target:

* 8 to 12 Major Rewards, primarily using existing spell content
* 10 to 15 Minor Rewards
* one Knowledge operation
* one Laboratory type
* fixed Laboratory pacing
* expedition-only progression

Suggested Minor Reward design categories:

* Combat
* Survival
* Knowledge
* Expedition

These categories are authoring aids.

They should not require a general classification framework unless implementation produces a concrete need.

## MVP UI

### Room Completion

Required information:

* three Minor Reward options
* concise effect descriptions
* one selection action

### HUD

Required information:

* current Knowledge
* clear Knowledge gain feedback

### Laboratory

Required information:

* current spell configurations
* three Major Reward options
* Analyze Further action and Knowledge cost
* destination spell selection when required
* modification confirmation

The MVP should not use:

* graph-based spell editing
* research project queues
* research progress percentages
* module crafting
* module fusion
* multiple Laboratory types
* Research rarity systems
* Codex integration
* Instability
* persistent Knowledge
* permanent numerical progression

## Validation Questions

The MVP must validate:

1. Do Minor Rewards create useful run identity without interrupting combat too often?
2. Do players understand the difference between expedition development and spell evolution?
3. Does Knowledge feel valuable because it improves future decisions?
4. Is reaching a Laboratory anticipated?
5. Does a Major Reward make the player want to immediately test the changed spell?
6. Are two or three combat rooms enough to understand a spell before changing it again?
7. Do Minor Rewards produce situational choices rather than obvious universal picks?
8. Can players describe the resulting run through both their spell configuration and expedition modifiers?

## Future Expansion Boundaries

Possible future expansions include:

* route choices that expose Laboratories or Research domains
* Knowledge used to reveal Tower information
* domain-specific Laboratories
* option preservation or focusing
* Minor Reward evolution
* modifier caps or replacement
* advanced spell-manipulation operations

Add these only when playtests reveal a concrete need.

Do not expand Research only to make the system appear more original or complex.
