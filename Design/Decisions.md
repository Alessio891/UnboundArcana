# Decision: Generic Modifier System Direction

Date:

2026-07-11

## Context

The existing SpellStats class was only a placeholder and did not represent a real stat system.

Runtime objects currently contain gameplay values directly:

Examples:

* Explosion radius
* Projectile speed
* Damage
* Lifetime

Modules modifying runtime objects directly creates problems:

* stacking rules
* ownership ambiguity
* future equipment/buff integration

## Decision

Introduce a generic stat and modifier system.

The system will support:

* Damage
* Size
* Speed
* Duration

Modifiers may originate from:

* Spell modules
* Equipment
* Player progression
* Buffs
* Debuffs
* Future systems

## Consequences

Positive:

* Runtime objects no longer need direct knowledge of modifier sources.
* Future progression systems can participate naturally.
* Modifier calculations become centralized.
* Debugging becomes possible because modifier sources remain identifiable.

Negative:

* More runtime calculation complexity.
* UI/debugging tools will eventually be required to explain final values.

---

# Decision: Module Identity vs Numeric Modifiers

Date:

2026-07-11

## Context

Turning every numerical adjustment into a separate module would make spell construction noisy and less understandable.

## Decision

Modules remain meaningful gameplay concepts.

Modules may provide modifiers internally.

Example:

```
Fire Module Level 3
```

instead of:

```
Fire Module
+
Fire Damage Modifier
+
Burn Duration Modifier
```

## Consequences

Positive:

* Better player readability.
* Spell creation feels like building magical concepts.
* Less spreadsheet-like complexity.

Negative:

* Module upgrade systems need future design.
* Some balancing decisions become module-specific.

---

# Decision: Spell Composition Tags

Date:

2026-07-11

## Context

Modules require a way to communicate compatibility and synergy without forcing rigid restrictions.

## Decision

Tags belong to authored spell components:

* Behaviors
* Modules

The final SpellInstance calculates its resulting tags.

Example:

```
Projectile
+
Explosion
+
Fire
```

creates:

```
Projectile
Explosion
Area
Fire
```

Tags are descriptive and support:

* compatibility
* synergy
* discovery

They are not intended to become a strict class hierarchy.

## Consequences

Positive:

* Supports emergent builds.
* Allows module interactions.
* Avoids excessive hard restrictions.

Negative:

* Some future edge cases may require additional compatibility rules.

---

# Decision: Chained Spell Modifier Propagation

Date:

2026-07-11

## Context

A spell can create another spell composition.

The child spell should have its own identity, but some effects may logically continue.

## Decision

Chained spells have:

* their own behavior
* their own modules
* their own tags

Modifier inheritance is explicit.

A chaining mechanic decides which modifiers propagate.

Example:

```
Cast Spell On Destroy

inherits:

Fire modifiers
```

rather than:

```
Copy entire parent spell state
```

## Consequences

Positive:

* Prevents hidden exponential scaling.
* Keeps ownership clear.
* Allows powerful intentional interactions.

Negative:

* More configuration is required for advanced chaining mechanics.
