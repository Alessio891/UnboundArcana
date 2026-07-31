# Stats System

Shared stat model for entities and spells.

## Keys

Stats use string identifiers.

Common stats use `StatKeys`; mechanic-specific/internal values may use local string keys.

Do not introduce global enum entries for every possible runtime value.

## StatCollection

Supports:

* base values
* modifiers
* removal by source
* effective value lookup

Modifiers:

```text
Flat        value += amount
Percent     value *= 1 + amount
Multiplier  value *= amount
```

Base values and modifiers have a source.

Removing a source must remove its contributions.

## Ownership

A `StatCollection` belongs to the runtime object/system whose state it represents.

Spell runtime stats belong to `SpellInstance`.

Entity stats belong to the entity runtime.

ScriptableObjects contribute configuration/base values; they do not own mutable runtime stat state.

## Usage

Prefer source-based modifiers for temporary effects such as statuses.

Do not create parallel stat systems for individual mechanics unless the common model cannot represent the requirement.
