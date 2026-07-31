# Unbound Arcana - Game Vision

## Identity

2D top-down action roguelike about magical experimentation through player-created spells.

Core fantasy:

> Create, test and evolve magic that feels personally discovered.

The player is an arcane researcher, not a collector of predefined spells or equipment.

The game is not a roguelike containing a spell-building system.

The roguelike structure exists to drive repeated magical experimentation.

## Core Loop

```text
Discover
-> Build / Modify Experiment
-> Test in Combat
-> Observe Strengths and Weaknesses
-> Research
-> Evolve Experiment
-> Face New Challenges
```

A run is a research expedition.

Completing the expedition is the immediate objective.

The primary player experience is discovering, constructing and testing increasingly distinctive magical systems.

## Core Pillars

### 1. Spell Creation and Research

The player creates a small number of evolving spell experiments.

Spells combine:

* Behavior
* Modules / magical principles
* Stats
* Runtime interactions
* Events

Research is a core gameplay system, not fiction layered over an upgrade screen.

The desired loop combines:

**Discovery**

* partially stochastic
* preserves roguelike unpredictability
* exposes the player to unexpected possibilities

**Experimentation**

* intentional
* lets the player influence, modify and develop discovered possibilities
* rewards understanding and planning

Randomness should create opportunities.

Player decisions should determine what to do with those opportunities.

A novice may accidentally discover an extremely powerful interaction.

An experienced player should be better at recognizing, predicting and pursuing such interactions.

### 2. Combat

Combat must be satisfying before build complexity is added.

Movement, casting, impact, enemy response and feedback should provide a strong baseline experience.

Combat should be:

* responsive
* readable
* movement-driven
* skill-based
* capable of controlled chaos

Player skill and build quality should both matter.

A strong build may compensate for weaker execution and strong execution may compensate for a mediocre build, but neither should make the other irrelevant.

Combat exists partly to reveal properties of the current experiment:

* strengths
* weaknesses
* coverage
* control
* targeting
* mobility
* adaptability

Enemy roles and combinations should create different tactical pressures rather than primarily scaling health and damage.

### 3. Tower Expedition

The Tower creates context, pressure and discovery for spell experimentation.

It should provide:

* combat challenges
* research opportunities
* route decisions
* anomalies
* major tests / Guardians
* procedural variation

Exploration focuses primarily on room progression and route choice rather than continuous open-dungeon exploration or heavy backtracking.

Route decisions should be meaningful but not required after every room.

Prefer occasional strategic junctions over constant interruption of combat flow.

Routes may expose different:

* risks
* research opportunities
* domains
* anomalies
* challenge types

The Tower may influence what opportunities are available but should not dictate a required build.

## Spell Model

A spell has one fundamental Behavior.

Examples:

* Projectile
* Beam
* Aura
* future authored Behaviors

Modules modify or interact with Behavior/runtime capabilities.

Modules should not replace the fundamental spell identity.

Example:

```text
Projectile
-> + Fire
-> + Split
-> + Explosion
```

Desired reaction:

> I created this.

Not:

> I found a stronger weapon.

## Build Philosophy

Builds should support:

* strong combinations
* unusual combinations
* specialization
* experimentation
* emergent interactions

Power is not only damage.

Build strength may come from:

* output
* control
* coverage
* targeting
* mobility
* adaptability
* interaction potential
* risk/reward

Prefer upgrades that change behavior or decisions over incremental numerical increases.

### Broken Builds

Exceptionally powerful builds are desirable when they emerge from meaningful combinations, investment, opportunity or tradeoffs.

Do not balance every combination toward identical output.

A successful run may occasionally produce a build that feels unfairly powerful in the player's favor.

Avoid universal dominant strategies that make experimentation unnecessary.

## Deterministic Magic

Core spell behavior should remain understandable and predictable.

The same configuration should produce consistent fundamental behavior.

Complexity comes from interactions, not arbitrary spell mutation.

Randomness may affect:

* available discoveries
* Tower structure
* encounters
* anomalies
* circumstances

It should not prevent the player from understanding their own spell.

## Challenges and Objectives

The long-term encounter direction includes multiple objective types rather than only eliminating all enemies.

Possible pressure models include:

* eliminate
* survive
* destroy / interrupt
* defend / control
* future objective types that test meaningfully different build properties

Do not add objective types only for content count.

Each should create a genuinely different gameplay problem.

Initial implementations may use a small number of objective archetypes while preserving this direction.

## Enemies

Enemy design should focus on roles and composition.

An enemy can:

* create melee pressure
* create ranged pressure
* restrict positioning
* punish a specific pattern
* force target prioritization
* test area coverage
* test focused damage
* create interaction with other enemies

Interesting encounters should emerge partly from combinations of complementary roles.

Enemies should challenge builds without creating hard counters that invalidate player experimentation.

## Guardians

Guardians are major tests of the current experiment and the Tower context.

They should test:

* player execution
* understanding of the build
* strengths and weaknesses of the current spell system
* adaptation to the current research/domain context

Avoid Guardians that are primarily large HP/DPS checks.

## Research

Research should allow meaningful manipulation of spell experiments.

Potential actions include:

* add
* remove
* replace
* transform
* redirect
* constrain
* expand

Exact mechanics remain to be designed.

Research should eventually provide more agency than choosing one random upgrade from a list.

Discovery and Research should coexist:

```text
Random / contextual opportunity
-> player recognizes possibility
-> research enables intentional development
```

Research is one of the systems that should receive disproportionate design and polish effort.

## Run Progression

Run power is temporary.

On run end or death, lose:

* current spell builds
* run upgrades
* temporary research
* run progression

Failure is part of the roguelike structure.

Do not introduce persistent mechanical power merely to soften run failure.

## Knowledge and Meta Progression

Player knowledge is a major form of long-term progression.

Experienced players become stronger because they:

* recognize useful components
* predict interactions
* understand tradeoffs
* plan build directions
* exploit opportunities more intentionally

The game may permanently record discovered:

* principles
* modules
* interactions
* observations

A Codex/knowledge system may expose information the player has discovered without granting direct power.

Mechanical unlocks may exist if they expand possibilities rather than create permanent numerical superiority, but their role requires further design.

Permanent rewards may include cosmetics such as:

* skins
* pets
* visual effects
* presentation unlocks

Cosmetic progression must not affect run power.

## Run Variety

Primary sources of run variation:

* spell build
* room/layout assembly
* anomalies/random events
* Guardians
* starting conditions/loadouts

Enemy composition, route opportunities and research availability may further vary runs.

Variety should primarily create different decisions and experiments rather than merely randomize content order.

## Player Loadout

Current design baseline:

* 2 active spell slots
* 1 passive spell slot

This remains a design hypothesis.

The goal is a small number of deep experiments rather than a large ability inventory.

## Instability

Instability is a future system for dangerous experimentation.

It may create:

* opportunity
* complexity
* risk
* unusual events

It should not primarily cause random spell failure or make spell behavior impossible to understand.

Implement only when deterministic spell composition and Research are mature enough to support it.

## Content Direction

Prefer:

* authored mechanics
* authored Behaviors and Modules
* authored enemy roles
* authored room sections
* procedural assembly
* systemic combinations

Avoid relying on procedural generation to invent entirely new mechanics or content.

## Priority

When development priorities conflict:

1. Spell Creation and Research
2. Combat feel and interaction
3. Tower Expedition
4. Meaningful decisions and build identity
5. Readability and feedback
6. Content breadth
7. Supporting/meta systems
8. Architectural sophistication

Supporting systems exist to strengthen the core pillars.

Fun is not equivalent to complexity.

Before adding a new mechanic, consider whether the problem can instead be solved through:

* timing
* feedback
* encounter composition
* spatial pressure
* objective design
* rewards
* existing interactions

## Success Criteria

Unbound Arcana succeeds when:

* baseline combat is enjoyable
* players understand the magical rules
* discovery creates surprise
* Research creates agency
* builds develop distinctive behavior
* Tower challenges reveal properties of those builds
* powerful emergent combinations feel discovered rather than prescribed
* experienced players benefit from understanding without excluding lucky discovery by newcomers
* players feel ownership over the resulting magic
* memorable runs are defined by what the player created and how it behaved
