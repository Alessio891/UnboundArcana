# Editor Tools

## Gameplay Asset Browser

Location:

Tools > Unbound Arcana > Gameplay Asset Browser


Purpose:

Quickly navigate gameplay ScriptableObjects.

Supported:

- Spell Modules
- Spell Behaviors
- Statuses
- Entities
- AI


Features:

- search
- category filtering
- asset preview
- embedded inspector
- select asset in project


---

# Design Philosophy

The tool intentionally reuses Unity inspectors.

It is not a replacement editor.

It improves navigation and discovery.


---

# Asset Creation Tools

There is also an editor generator tool.

Purpose:

Create paired runtime/configuration classes.

Examples:

Module:

ModuleDefinition
Module


Behavior:

BehaviorDefinition
Behavior


The tool can also create ScriptableObject assets after compilation.