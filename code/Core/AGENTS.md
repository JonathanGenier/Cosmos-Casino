# Cosmos Casino Core — Agent Instructions

## Architectural Role

`CosmosCasino.Core` is the authoritative game simulation layer.

Core defines the logical state and behavior of the game independently from Godot, rendering, UI, input devices, scenes, or other presentation concerns.

If Core and Client disagree about gameplay state, Core is authoritative.

Core should remain usable and testable without launching Godot.

---

## Core Owns

Core is responsible for authoritative game concepts such as:

* Game state
* Simulation state
* Map state and rules
* Terrain state and rules
* Build validation and execution
* Domain-specific coordinate systems and conversions
* Player intents and logical actions
* Deterministic procedural generation
* Save/load-relevant state
* Gameplay validation
* Logical events/state changes

As new gameplay systems are added, authoritative state and rules should generally live in Core unless there is a clear architectural reason otherwise.

---

## Core Does Not Own

Core must not contain presentation or engine-specific concerns.

Do not introduce:

* Godot nodes
* Godot scenes
* Godot resources
* Godot vectors or math types
* Rendering logic
* Mesh generation
* Materials or shaders
* UI state
* Camera behavior
* Input polling
* Godot lifecycle methods
* Visual previews
* Animation state

Those responsibilities belong in Client.

---

## Engine Independence

Core must have zero dependency on Godot.

Do not reference the `Godot` namespace from Core.

Use:

* standard .NET types
* Core-owned value types
* domain-specific coordinate types
* engine-agnostic abstractions

Client is responsible for converting Core data into Godot-specific representations.

---

## Determinism

Authoritative Core behavior should be deterministic wherever practical.

Given the same:

* initial state
* seed
* intents
* simulation inputs

Core should produce the same logical result.

Do not make authoritative behavior depend on:

* frame rate
* rendering state
* Godot lifecycle
* wall-clock timing unless explicitly modeled
* uncontrolled randomness

Randomness affecting authoritative state must be controlled by Core.

---

## Save / Load Compatibility

Authoritative Core state should be designed so it can eventually be saved and restored.

Avoid authoritative state that depends on:

* runtime-only Godot objects
* scene references
* Client-side objects
* event subscribers
* non-reconstructable external state

Core state should represent the logical game, not its current visual representation.

---

## Domain Modeling

Prefer explicit domain types when they prevent invalid or ambiguous usage.

Examples include coordinate types such as:

* `MapCoord`
* `TerrainTileWorldCoord`
* `TerrainChunkLocalCoord`
* `TerrainChunkGridCoord`

Do not replace strong domain types with generic vectors solely for convenience.

Two types may contain the same underlying values while representing different coordinate spaces or domains.

Keep those distinctions explicit.

---

## Coordinate Conventions

Core owns authoritative spatial semantics.

Coordinate conversions should be centralized in the appropriate Core math/conversion layer.

Do not duplicate authoritative coordinate formulas throughout unrelated systems.

### World Origin

The intended convention is:

**World X/Z `(0,0)` is the center of logical cell/tile `(0,0)`.**

Logical world coordinates may be negative.

Do not assume the logical map begins at `(0,0)` and expands only into positive coordinates.

Local storage/index coordinates may remain zero-based when appropriate.

Keep world, chunk, local, and map coordinate spaces distinct.

---

## Map and Terrain

Map and Terrain are separate domains even when they align spatially.

Do not couple them unnecessarily.

If a `MapCoord` and `TerrainTileWorldCoord` refer to the same physical grid location, preserve the separate domain types and convert explicitly at the correct integration boundary.

Do not merge domains merely because their coordinates share the same numeric values.

---

## Ownership and Responsibility

Keep authoritative rules close to the domain object or system that has the information required to enforce them.

Do not move rules upward into managers purely for convenience.

Managers should generally coordinate systems and operations rather than absorb every domain invariant.

Storage classes should primarily own storage concerns.

Avoid multiple mutable sources of truth for the same authoritative state.

---

## Intents and State Mutation

Player actions should flow through explicit logical operations.

Prefer a flow such as:

`Intent -> Validation -> Execution -> Core state mutation`

Client may request an action or display a preview, but Client must not directly determine authoritative gameplay outcomes.

Where intents are used, keep them engine-agnostic and preferably immutable.

---

## Validation

Gameplay validation belongs in Core.

Client may display validation results but must not duplicate authoritative gameplay rules.

Expected gameplay failures should use domain validation results where appropriate rather than exceptions.

Exceptions should generally represent programmer errors or violated invariants.

---

## Testing Convention

Core must remain highly testable.

Tests use NUnit and should target Core without requiring Godot.

### When changing Core behavior

Always:

1. Add or update relevant tests.
2. Run the affected Core test suite.
3. Test both successful and invalid cases where applicable.
4. Test edge conditions for coordinate and bounds logic.
5. Include negative-coordinate cases when spatial behavior is involved.
6. Preserve deterministic test behavior.

Do not weaken existing tests merely to make a change pass.

If a test exposes a conflict with an architectural invariant, investigate the behavior before changing the expected result.

New authoritative Core behavior should normally be accompanied by tests.

---

## Refactoring Convention

Keep changes scoped to the requested task.

Before modifying an existing Core system:

1. Inspect the implementation.
2. Inspect its callers.
3. Inspect related domain types.
4. Inspect existing tests.
5. Understand the current ownership and dependency direction.

Do not perform unrelated cleanup or redesign unless required for correctness.

Do not replace deliberate project architecture with generic patterns simply because they are conventional.

Prefer removing duplicated authoritative logic over introducing another abstraction around it.

---

## Performance

Performance matters, especially as simulation scale grows, but correctness and maintainability come first.

Avoid speculative optimization.

Before introducing complex optimization, consider:

* algorithmic complexity
* allocation frequency
* hot-path frequency
* data locality
* collection size
* determinism
* maintainability

Do not introduce concurrency, pooling, unsafe code, or complex caching without a concrete reason.

---

## Agent Expectations

When working in Core:

* Preserve Core authority.
* Preserve engine independence.
* Preserve determinism.
* Preserve domain boundaries.
* Add or update tests for behavior changes.
* Avoid unrelated refactors.
* Do not silently invent architectural conventions.
* Report important assumptions or ambiguities.
* Build and run relevant tests after implementation.

If a requested change would violate these rules, identify the conflict rather than silently implementing the violation.

---

## Guiding Principle

Core should represent the complete logical game independently from its presentation.

Prefer an explicit, testable, authoritative design over a convenient shortcut that leaks presentation concerns or weakens domain boundaries.
