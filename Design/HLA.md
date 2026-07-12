# High Level Architecture

## Complete System Overview

```mermaid
flowchart TD

    subgraph Editor["Editor Configuration Layer"]

        SD["SpellDefinition<br/>ScriptableObject"]

        SC["SpellConfiguration<br/>Player Owned"]

        SBD["SpellBehaviorDefinition"]

        SMD["SpellModuleDefinition[]"]

    end


    subgraph Runtime["Spell Runtime Layer"]

        SF["SpellFactory"]

        SI["SpellInstance"]

        B["SpellBehavior"]

        M["SpellModules"]

        RO["SpellRuntimeObjects"]

        SEC["SpellEventBus"]

        SRC["SpellRuntimeContext"]

        STATS["SpellStatCollection"]

    end


    subgraph Events["Event Layer"]

        GE["GameEventBus"]

    end


    subgraph Gameplay["Gameplay Systems"]

        DMG["Damage System"]

        ENEMY["Enemy System"]

        REWARD["RewardController"]

        WAVE["EnemyWaveSpawner"]

    end


    SD --> SBD
    SD --> SMD

    SC --> SF

    SF --> SI

    SI --> B
    SI --> M
    SI --> RO
    SI --> SEC
    SI --> SRC
    SI --> STATS


    B --> RO

    M --> SEC
    RO --> SEC

    SEC --> GE

    GE --> DMG
    GE --> ENEMY
    GE --> REWARD
    GE --> WAVE


    REWARD --> SC
