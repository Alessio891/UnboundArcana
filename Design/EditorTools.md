# Editor Tools

Editor tooling lives in `UnboundArcana.Editor`.

Runtime code must not depend on `UnityEditor`.

## Gameplay Assets

Tools exist for browsing and creating gameplay ScriptableObjects and paired runtime/configuration types.

Use existing asset tooling before introducing parallel creation workflows.

## Room Authoring

Room-section editor tooling supports operations including:

* section setup
* connectors
* footprints
* markers
* Tilemap setup/normalization
* grid alignment
* prop/reference gathering
* authoring visualization

`RoomSection` remains Runtime-safe.

Authoring operations belong in Editor code, not runtime components.

## Principles

* Editor tools assist authoring; runtime components remain authoritative runtime data.
* Prefer existing Unity inspectors where sufficient.
* Keep destructive/editor-only operations outside Runtime.
* Do not add runtime complexity solely to simplify an editor workflow when an Editor-only solution is possible.

For implementation details inspect the relevant Editor classes; this document is only an ownership/workflow guide.
