# Decisions

Compact record of intentional project decisions.

Consult when reconsidering an established architectural or design boundary.

Current code defines implementation. `GameVision.md` defines desired player experience. This file records decisions likely to be reconsidered later.

## Spell Execution Ownership — 2026-07-12

`SpellConfiguration` owns persistent composition.

`SpellInstance` represents one execution and is created per cast.

Reason: prevent runtime state leaking between casts and keep configuration changes limited to future executions.

## Behaviors Define Spell Identity — 2026-07-13

A Behavior defines the fundamental way a spell exists.

Modules extend that identity; they do not replace it.

Reason: keep composition understandable and prevent proliferation of specialized spell types.

## Capability-Based Module Extension — 2026-07-13

Modules should target runtime capabilities rather than concrete runtime object types when the mechanic is genuinely reusable.

Concrete coupling is acceptable for genuinely type-specific mechanics.

Reason: enable reuse without speculative abstraction.

## Runtime Object Extension Is General — 2026-07-13

Runtime-object modification is not a projectile-only extension point.

Projectile, Aura, Beam and future Behaviors may expose appropriate capabilities.

## Aura Terminology — 2026-07-13

`Aura` refers to a spell Behavior creating a persistent area/runtime object.

Entity buffs/debuffs belong to the Status system.

## No Predefined Synergy System — 2026-07-13

Do not create a general system of hardcoded module recipes/synergies.

Prefer emergent results from Behaviors, Modules, runtime interactions and explicit gameplay state.

Compatibility rules may constrain invalid combinations without defining optimal recipes.

## Tags Only When Needed — 2026-07-13

Do not introduce a broad tag framework until real module/content diversity produces concrete compatibility or classification problems.

Solve demonstrated requirements, not hypothetical ones.

## MVP Spell Prototype Complete — 2026-07-13

Spell architecture and player-driven spell modification were sufficiently validated.

Development focus moved from proving spell architecture to building the actual game experience.

## Three Core Pillars — 2026-07-31

Development priorities are centered on:

1. Spell Creation and Research
2. Combat
3. Tower Expedition

Other gameplay, content and meta systems should primarily support these pillars.

Reason: prevent secondary systems from consuming scope without strengthening the core experience.

## Research Is a Core System — 2026-07-31

Research must become more than a themed random-upgrade screen.

Desired model combines:

* stochastic/contextual Discovery
* intentional Experimentation

Randomness produces opportunities; Research provides agency over how discoveries develop.

Exact mechanics remain open.

## Combat Must Work Without Build Complexity — 2026-07-31

Baseline movement, casting, impact and enemy interaction should be satisfying before complex spell combinations are considered.

Spell progression amplifies a strong combat foundation; it must not compensate for weak fundamentals.

## Player Skill and Build Both Matter — 2026-07-31

Neither mechanical execution nor build quality should fully dominate.

Strong execution may compensate for a weaker build and a strong build may compensate for weaker execution.

Both remain meaningful throughout a run.

## Powerful Emergent Builds Are Valid — 2026-07-31

Runs may produce exceptionally powerful or effectively "broken" builds.

This is desirable when power emerges from combinations, investment, opportunity or tradeoffs.

Do not normalize every build to equivalent output.

Prevent repeatable universal dominant strategies rather than exceptional run-specific power.

## Tower Challenges Test Builds — 2026-07-31

Enemy roles, encounter composition, objectives and Guardians should expose different properties of player builds.

Long-term direction includes objective variety beyond eliminating all enemies.

Avoid hard counters that invalidate legitimate experiments.

## Tower Routing Uses Occasional Strategic Choice — 2026-07-31

Tower progression focuses on rooms and route decisions rather than open-dungeon exploration.

Do not require a routing decision after every room.

Prefer occasional meaningful junctions that influence risk, research opportunities, domains or anomalies without constantly interrupting gameplay flow.

## Run Mechanical Power Resets — 2026-07-31

Death or run completion ends the current mechanical build/progression.

Do not use persistent numerical power to remove roguelike consequences.

Permanent progression may record knowledge, expand possibilities or unlock cosmetics, but should not make repeated runs easier through automatic stat growth.

## Knowledge Is Player Mastery — 2026-07-31

Understanding the spell system is an important form of progression.

Experienced players should better predict and pursue builds.

New players must still be able to discover powerful interactions accidentally.

A future Codex may record discovered information without replacing player understanding.

## Cosmetic Persistence Is Allowed — 2026-07-31

Permanent cosmetic rewards may survive runs.

Examples include skins, pets and visual effects.

They must not affect gameplay power.

## Run Progression Uses Two Scales — 2026-08-01

Run progression distinguishes between:

```text
Minor Rewards
-> expedition-level development

Major Rewards
-> significant spell evolution
```

Minor Rewards should not constantly modify spell composition.

Major Rewards occur less frequently so the player has time to understand and use the current spell configuration.

Reason: provide progression at multiple rhythms while preserving spell identity and avoiding a spell-editing interruption after every room.

## Minor Rewards Activate Immediately — 2026-08-01

Minor Rewards become active when selected and remain active for the expedition.

The initial design does not use active research slots, passive progress bars or delayed Minor Reward activation.

Reason: passive completion added delayed feedback without introducing enough meaningful player interaction.

## Knowledge Increases Research Agency — 2026-08-01

Knowledge is a temporary expedition resource earned through normal Tower play.

Its primary role is to improve player agency during Major Research, such as revealing additional Major Reward options.

Knowledge should not be required to access the baseline Major Reward and should not primarily purchase direct numerical power.

Reason: connect Combat and Tower progression to spell evolution without turning Knowledge into generic experience or a mandatory power currency.

## Major Research Occurs at Laboratories — 2026-08-01

Significant spell evolution occurs at infrequent Laboratory moments.

Laboratories present Major Reward opportunities and apply the selected modification immediately.

The following combat rooms allow the player to test and understand the changed spell.

Reason: create readable and anticipated spell-evolution moments while preserving combat pacing between modifications.

## Research MVP Parameters Are Provisional — 2026-08-01

The following are prototype parameters rather than permanent design commitments:

* three Minor Reward options
* one Minor Reward after each standard room
* three initial Major Reward options
* one additional option revealed through Knowledge
* approximately two Laboratories per floor
* fixed Laboratory placement
* current reward-pool sizes and Knowledge costs

These values should change according to playtest results without being treated as reversals of the core Research direction.


## Decision Policy

Add an entry when a decision:

* affects architectural ownership
* rejects a plausible alternative likely to return
* constrains future system design
* protects a core game-design principle

Do not record ordinary implementation choices.
