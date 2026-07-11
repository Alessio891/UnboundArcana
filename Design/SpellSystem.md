# Spell System Design

## Core Concept

A spell is not a class.

A spell is a composition of:

```
Behavior
+
Stats
+
Modules
+
Events
```

Example:

```
Projectile

Fire Module

Explosion Module
```

Creates:

```
Explosive fire projectile
```

No custom spell class is required.

---

# Spell Components

## Behavior

Defines how the spell exists.

Examples:

- Projectile
- Beam
- Aura
- Trap
- Minion
- Meteor
- Nova
- Wall
- Orbit


A behavior controls:

- spawning
- movement
- lifetime
- runtime object creation
- interpretation of spell lifecycle commands


Behaviors also provide default stats required for their existence.

Examples:

Projectile:

```
Speed
Duration
```

Aura:

```
Duration
```


A behavior should not know about effects.

Example:

Projectile:

Knows:

- movement
- collision
- lifetime

Does not know:

- Fire exists
- Explosion exists
- Damage exists


---

# Modules

Modules modify or extend spell behavior.

Modules communicate through events.

Modules can:

- react to spell lifecycle events
- create effects
- contribute stats
- extend gameplay behavior


Categories:

---

## Behavior Modules

Modify movement or existence.

Examples:

- Bounce
- Split
- Pierce
- Homing
- Orbit
- Accelerate
- Return


---

## Effect Modules

Modify spell results.

Examples:

- Fire
- Ice
- Poison
- Lightning
- Burn
- Freeze
- Knockback
- Lifesteal


---

## Trigger Modules

Create additional events or effects.

Examples:

- Explosion
- Spawn Minion
- Spawn Aura
- Chain Lightning
- Repeat Cast


---

# Stats

Stats are owned by the runtime spell composition.

The current implementation uses:

```
SpellInstance

    |
    v

StatCollection
```


Stats are created from multiple sources:

- Behavior definitions
- Module definitions
- Future player progression
- Future equipment
- Future buffs/debuffs


Current stats:

- Damage
- Size
- Speed
- Duration


The meaning of a stat depends on the component using it.

Examples:

Projectile:

```
Speed = movement speed
Duration = lifetime
Size = projectile size
```

Aura:

```
Duration = active time
Size = radius
```

Explosion:

```
Size = explosion radius
Damage = damage dealt
Duration = active lifetime
```


Runtime objects query effective values.

They do not know which component created the modifiers.


---

# Modifier System

Modifiers represent changes to stats.

Supported operations:

- Flat
- Percent
- Multiplier


A modifier contains:

- Stat identifier
- Value
- Operation
- Source


Example:

```
Fire Module Level 3

adds:

Damage +30%
```

The runtime aggregates modifiers instead of modules directly changing runtime object fields.


---

# Runtime Objects

Runtime objects represent gameplay entities created by behaviors or modules.

Examples:

- Projectile
- Explosion
- Aura
- Beam
- Trap
- Minion


Runtime objects own:

- gameplay state
- lifetime
- updates
- interaction with the world


Runtime objects can query effective spell values.

Example:

```
ExplosionRuntimeObject

asks:

SpellInstance Stats

for:

Size
Damage
Duration
```


Runtime objects do not know:

- which module created a modifier
- which behavior exists
- how the spell was configured


---

# Spell Chaining

A chained spell is a new spell composition.

Example:

Parent:

```
Projectile

+

Ice Module

+

Explosion
```

creates:

Child:

```
Projectile

+

Fire Module
```


The child has:

- its own behavior
- its own modules
- its own stats
- its own tags
- its own runtime objects


Modifier inheritance is explicit.

A chaining mechanic decides what is transferred.

Example:

```
CastSpellOnDestroyModule

inherits:

Fire modifiers

does not inherit:

unrelated modifiers
```


---

# Tags

Tags describe spell composition.

Tags originate from:

- Behavior definitions
- Module definitions


The final SpellInstance calculates its resulting tags.


Tags support:

- compatibility
- synergy
- discovery
- rewards
- UI


Tags are descriptive.

They are not intended to become a strict class system.


---

# Important Rules

1. No Fireball class.
2. No IceProjectile class.
3. Spells are compositions.
4. Modules must remain independent.
5. Behaviors must not know modules exist.
6. Modules do not communicate directly.
7. Runtime objects own gameplay state.
8. ScriptableObjects contain configuration only.
9. Views only represent runtime objects.
10. Stats are created by spell composition.
11. Modifier ownership remains identifiable.
12. New combinations should emerge naturally.