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

* Projectile
* Beam
* Aura
* Minion
* Trap
* Meteor
* Nova
* Wall
* Orbit

A behavior controls:

* spawning
* movement
* lifetime
* existence

A behavior should not know about effects.

Example:

Projectile:

* move
* detect collision
* emit Hit event

Projectile does not know:

* Fire exists
* Explosion exists
* Damage exists

---

# Modules

Modules modify or extend spell behavior.

Categories:

---

## Behavior Modules

Modify movement/existence.

Examples:

* Bounce
* Split
* Pierce
* Homing
* Orbit
* Accelerate
* Return

---

## Effect Modules

Modify results.

Examples:

* Fire
* Ice
* Poison
* Lightning
* Burn
* Freeze
* Knockback
* Lifesteal

---

## Trigger Modules

Create additional events.

Examples:

* Explosion
* Spawn Minion
* Spawn Aura
* Chain Lightning
* Repeat Cast

---

# Events

Modules communicate through events.

Implemented:

* Cast
* Hit
* Damage

Planned:

* Spawn
* Move
* Kill
* Expire
* Tick
* Destroy

Example:

```
Projectile

OnHit

Explosion Module reacts
```

Projectile does not know Explosion exists.

---

# Stats

All spells use shared stats.

Examples:

* Damage
* Speed
* Size
* Range
* Duration
* Cooldown
* Lifetime
* Knockback
* Pierce
* Chain
* Crit Chance
* Crit Damage
* Mana Cost
* Spawn Count

The meaning depends on the behavior.

Example:

Projectile:

```
Speed = movement speed
Range = lifetime distance
Size = collider size
```

Aura:

```
Size = radius
Speed = tick rate
Duration = active time
```

---

# Spell Fusion

Fusion combines spell graphs.

Example:

Spell A:

```
Projectile
Explosion
```

Spell B:

```
Aura
Poison
```

Possible result:

```
Projectile

OnHit

Spawn Aura

Aura

Poison
```

---

# Important Rules

1. No Fireball class.
2. No IceProjectile class.
3. Modules must be independent.
4. Behaviors must not know modules exist.
5. New combinations should emerge naturally.
