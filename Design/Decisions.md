# Design Decisions

---

# 2026-07-09

## Project Name

Decision:

The project name is:

Unbound Arcana

Reason:

The name represents:

* unlimited magical creation
* freedom from predefined spells
* experimentation and discovery

---

# Engine

Decision:

Use Unity 6000.3.19f1 LTS.

Reason:

Best fit for:

* 2D development
* ScriptableObjects
* Editor tooling
* data-driven architecture

---

# Spell Architecture

Decision:

Use composition instead of inheritance.

Reason:

Avoid exponential class growth.

Rejected:

Creating individual spell classes.

Example:

Rejected:

```text
Fireball.cs
IceProjectile.cs
ExplosiveArrow.cs
```

Preferred:

```text
Projectile
+
Fire Module
+
Explosion Module
```

---

# Spell Representation

Decision:

A spell is a graph of behaviors and modules.

Reason:

Allows procedural combinations.

---

# Runtime Separation

Decision:

Separate editor data from runtime objects.

Reason:

ScriptableObjects should contain configuration only.

Runtime objects should be created during gameplay.

Example:

SpellDefinition:

```text
Projectile Behavior
Fire Module
Damage 10
```

Runtime:

```text
ProjectileBehavior instance
FireModule instance
Current cooldown
Active projectiles
```

---

# Event Communication

Decision:

Modules and behaviors communicate through events.

Reason:

Behaviors should not know modules exist.

Example:

Projectile:

```text
Collision
    |
    v
HitEvent
```

Fire module:

```text
HitEvent
    |
    v
Apply fire effect
```

---

# First Development Goal

Decision:

Build spell sandbox before full game.

Reason:

The spell system is the main gameplay feature.
