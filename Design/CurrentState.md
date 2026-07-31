# Current State

Current development snapshot.

Update when project direction, milestone or major implemented systems change.

## Milestone

The spell architecture prototype is complete.

Current milestone:

> Build and validate the first complete playable Tower expedition.

The goal is a coherent run that can be played by someone unfamiliar with the project.

After the end-to-end loop works, development should focus on validating the three core pillars:

1. Spell Creation and Research
2. Combat
3. Tower Expedition

## Target Game Flow

```text
Main Menu
-> Intro
-> Tower Expedition
-> Floors / Rooms
-> Expedition Complete or Player Death
```

Main Menu and Intro exist.

Tower/Expedition flow is under active development.

## Spells

Core composition architecture is implemented.

Foundation includes:

* player-owned configurations
* spell slots
* per-cast runtime instances
* cast time
* per-slot cooldown
* held-input repeated casting
* runtime stats/events
* runtime objects/views
* module composition

Validated Behaviors include:

* Projectile
* Aura
* Beam

Modules may affect:

* stats
* spell events
* runtime-object behavior/capabilities

Architecture is considered a stable foundation, not the current primary design problem.

## Research and Run Progression

The current reward infrastructure can modify player spell configurations during an expedition.

The intended MVP progression model now separates three responsibilities:

```text
Minor Rewards
-> immediate expedition-level Run Modifiers

Knowledge
-> temporary resource that increases Research agency

Major Rewards
-> significant spell changes at Laboratories
```

Minor Rewards do not directly modify spell composition.

They activate immediately and remain active for the expedition.

Knowledge is earned through normal Tower play and is spent at Laboratories to improve the available Major Reward decision.

The initial Laboratory operation reveals one additional Major Reward option.

Planned first version:

* one Minor Reward choice after standard rooms
* immediate Minor Reward activation
* Knowledge earned through rooms, objectives and combat
* fixed Laboratory moments
* three Major Reward options
* one Knowledge action that reveals an additional option
* immediate Major Reward application
* no research queues or progress bars

This remains a design hypothesis requiring implementation and playtesting.

Primary validation risks:

* Minor Rewards may interrupt room pacing
* Minor Rewards may become generic stat inflation
* accumulated Run Modifiers may become difficult to read
* Knowledge may feel like an unnecessary intermediary currency
* Laboratories may be too frequent or too rare
* Major Reward offers may not provide enough useful agency
* spell configurations may change before players have time to understand them

See `Research.md` for the current design, MVP boundaries and validation criteria.


## Combat / Entities

Implemented foundation includes:

* entities
* stats
* health/damage
* statuses
* movement
* casting
* sensing/AI
* melee combat
* gameplay event flow

Combat functionality exists but game feel and tactical depth remain prototype quality.

Current priorities include:

* movement/collision feel
* attack commitment/readability
* enemy feedback
* casting/hit feedback
* enemy role differentiation
* encounter composition

Baseline combat must become satisfying independently of complex spell progression.

## Enemies and Objectives

Enemy system foundation exists.

Current enemy behavior/content does not yet provide enough tactical variety.

Desired direction:

* distinct enemy roles
* complementary enemy combinations
* different forms of pressure
* encounters that expose build strengths/weaknesses

Room objectives currently remain limited.

Long-term direction includes objective variety beyond elimination, starting with a small set of distinct pressure models rather than many content variants.

## Expedition

Implemented foundation includes:

* expedition lifecycle
* player expedition coordination
* floor progression
* room generation/transition
* room completion flow
* encounter/objective integration
* research/reward integration

The complete end-to-end Tower experience is still being developed.

Death/end-of-run mechanical state is temporary by design.

## Rooms and Tower

Implemented authored/procedural hybrid foundation:

* authored room sections
* section footprints
* connectors
* procedural section assembly
* room markers
* Tilemap-based environment
* runtime room flow
* editor authoring tools

The Tower uses authored content assembled procedurally rather than fully generated mechanics/content.

Intended exploration model:

* room progression
* occasional route choices
* limited interruption of action flow

Future route choices may influence:

* research opportunity
* risk
* domain
* anomaly
* challenge type

Route planning should provide agency without requiring a decision after every room.

## Guardians

Guardian/boss design is not mature.

Desired role:

> major test of the current experiment and player execution

Guardians should test build properties and adaptation rather than act primarily as HP/DPS checks.

## Run Progression

Mechanical power gained during the run is temporary.

Current direction does not use permanent numerical power progression.

Long-term persistent progression may include:

* recorded knowledge/discoveries
* possibility-expanding unlocks, if justified
* cosmetics

Cosmetic rewards may persist without affecting gameplay.

Knowledge/mastery should make experienced players better at recognizing and planning builds without preventing accidental powerful discoveries by new players.

## Current Strengths

Validated:

* spell composition architecture
* separation of configuration and execution
* emergent module combinations
* spell evolution during gameplay
* authored + procedural room approach
* expedition/room technical foundation
* extensible gameplay/event boundaries

## Current Weaknesses

Highest risks:

* Research is not yet deep enough for its intended pillar status
* combat feel needs substantial polish
* enemy roles/compositions lack depth
* objectives lack variety
* reward/build choices need greater qualitative variety
* full-run pacing is unvalidated
* Tower routing/strategic choices are not yet mature
* Guardians are not yet defined as build tests
* UI/presentation remain incomplete
* content volume is low

## Active Priority

Finish a coherent playable expedition before broad content production.

Priority:

1. Complete end-to-end expedition flow.
2. Improve baseline combat feel.
3. Establish useful enemy/encounter pressure.
4. Implement and validate the MVP Research and run-progression loop defined in Research.md.
5. Add enough objective/route variety to test Tower pacing.
6. Playtest complete runs.
7. Expand content around validated systems.
8. Add supporting/meta systems only when required.

Do not solve missing gameplay depth by expanding architecture by default.

## Not Yet Mature

Treat these as prototype/future-design areas unless explicit implementation/design says otherwise:

* Research mechanics and balancing
* Minor Reward content
* Knowledge economy
* Laboratory pacing and offer generation
* build compatibility and reward generation
* combat balancing
* enemy design
* encounter/objective design
* reward generation
* build compatibility/restrictions
* Tower routing
* Guardians
* pacing
* UI
* meta progression
* knowledge/Codex system
* instability
* content scale
