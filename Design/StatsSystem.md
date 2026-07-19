# Stats System

## Philosophy

Stats are not enums.

They are string identifiers.

Reason:

The game contains many emergent mechanics.

Not every value deserves a global enum entry.

Example:

Permanent:

move_speed

Dynamic:

freeze_buildup


---

# Stat Keys

Common stats use constants.

Example:

StatKeys.Entity.MoveSpeed

StatKeys.Spell.Damage


Internal mechanics may use strings directly:

"freeze_buildup"


---

# StatCollection

Shared by:

- spells
- entities


Supports:

AddBase()

AddModifier()

RemoveModifiersFromSource()

Get()


---

# Modifier Operations

Flat:

value += amount


Percent:

value *= 1 + amount


Multiplier:

value *= amount


---

# Status Usage

Statuses can modify stats.

Example:

Frozen:

MoveSpeed
Multiplier
0.5


When the status expires:

RemoveModifiersFromSource(statusInstance)

