# Technical Architecture

## Engine

Unity 6000.3.19f1 LTS

Project type:

2D

Initial rendering:

Built-in Render Pipeline

---

# Architecture Style

Data-driven composition.

Avoid deep inheritance.

Prefer:

- ScriptableObjects
- Composition
- Interfaces
- Events

---

# Main Systems


SpellSystem

CharacterSystem

EnemySystem

RoomSystem

LootSystem

UISystem

SaveSystem


---

# Spell Runtime Architecture

## SpellDefinition

Static data.

Likely ScriptableObject.

Contains:

- Behavior definition
- Initial stats
- Module definitions
- Visual information


---

## SpellInstance

Runtime object.

Contains:

- Current stats
- Owner
- Runtime modules
- Current state


---

## SpellContext

Shared information.

Contains:

- Stats
- Owner
- Position
- Direction
- Random state
- Event system


---

# SpellFactory

Responsible for:


SpellDefinition

↓

SpellInstance

↓

Initialize behavior

↓

Initialize modules


---

# Event System

Each spell has an event bus.

Example:


Projectile

Collision

↓

Hit Event

↓

Modules react


---

# Development Tools

Important early tool:

Spell Sandbox.

Features:

- Spawn spells
- Spawn enemies
- Modify stats
- Test combinations

This should exist before final UI.

---

# Folder Structure

Suggested:


Assets

Scripts

Core

SpellSystem

    Runtime
    Behaviors
    Modules
    Events
    Stats

Character

Enemy

Rooms

Loot

UI

ScriptableObjects

Prefabs

Art

Audio