# Cosmos Casino Client — Agent Instructions

## Architectural Role

`CosmosCasino.Client` is the Godot-facing presentation and integration layer.

Client is responsible for translating authoritative Core state into:

* Visuals
* UI
* Input interactions
* Camera behavior
* Build previews
* Terrain rendering
* Godot nodes and scenes
* Audio and animation
* Other presentation-specific behavior

Client represents the game.

It does **not** define what the game logically is.

---

## Core Is Authoritative

`CosmosCasino.Core` is the authoritative simulation.

If Client and Core disagree about:

* Game state
* Map state
* Terrain state
* Build validity
* Object existence
* Coordinate semantics
* Simulation results

Core is correct.

Client must adapt to Core.

Do not modify or duplicate authoritative Core rules merely to make Client implementation easier.

---

## Dependency Direction

The intended dependency direction is:

`Client -> Core`

Client may reference Core.

Core must never reference Client.

Do not introduce circular dependencies between Client and Core.

---

## Client Owns

Client is responsible for presentation and engine integration concerns such as:

* Godot node lifecycle
* Scene instantiation
* Rendering
* Mesh generation
* Materials
* Shaders
* Visual positioning
* UI
* Camera
* Cursor representation
* Build previews
* Terrain views
* Input surfaces
* Animation
* Audio
* Visual effects
* Translating Core values into Godot types

These concerns should remain disposable and reconstructable from authoritative Core state where practical.

---

## Client Does Not Own

Client must not become authoritative for gameplay.

Do not place the following responsibilities in Client:

* Gameplay state
* Map rules
* Terrain rules
* Build rules
* Authoritative placement validation
* Simulation logic
* Save-game state
* Domain invariants
* Authoritative coordinate semantics
* Procedural generation state
* Logical action execution

If logic determines what **can**, **cannot**, **does**, or **does not** happen in the simulation, it likely belongs in Core.

---

## Disposable Representation

Client objects should generally be treated as disposable representations of Core state.

A Client node should be able to be:

* destroyed
* recreated
* rebound
* visually rebuilt

without changing authoritative gameplay state.

Do not rely on the continued existence of a particular Godot node to keep the simulation correct.

---

## Godot Lifecycle

Godot lifecycle methods are integration hooks, not authoritative simulation drivers.

Methods such as:

* `_Ready`
* `_Process`
* `_PhysicsProcess`
* `_Input`
* `_UnhandledInput`

may be used when appropriate for Client concerns.

They must not become the source of truth for logical game progression.

Avoid placing simulation behavior inside `_Process`.

Prefer event-driven or explicit flow-driven behavior for game interactions.

---

## Game Flows

GameFlows are event-driven.

Do not turn GameFlows into continuously polling Godot `_Process` systems without a clear presentation-specific reason.

Flows should coordinate interactions and transitions rather than own authoritative game state.

Godot lifecycle should initialize or integrate flows, not define their logical truth.

---

## Input

Input is centralized and context-aware.

Do not introduce isolated systems that react globally to input without participating in the established input architecture.

Input handling should respect:

* Active interaction context
* Priority
* Modal state
* Cancel / Escape arbitration
* Current player interaction mode

Avoid multiple unrelated systems independently reacting to the same input action.

---

## Player Actions

Client captures player intent.

Client should not directly mutate authoritative state.

The intended architectural flow is:

`Player input`
→ `Client interaction`
→ `Core intent / operation`
→ `Core validation`
→ `Core execution`
→ `Core state mutation`
→ `Client visual update`

Client may initiate the request.

Core decides the authoritative outcome.

---

## Build Previews

Build previews are visual representations of a potential Core action.

Client may:

* render a preview
* highlight affected cells
* show placement validity
* update preview geometry
* follow the cursor

Client must not duplicate authoritative build rules.

If placement validity depends on gameplay state, obtain that result from Core.

Preview logic should not become a second implementation of build validation.

---

## Coordinates

Core owns authoritative coordinate semantics and conversions.

Client may convert Core values into Godot representations such as `Vector2`, `Vector2I`, `Vector3`, or `Vector3I`.

Client must not independently redefine map or terrain coordinate rules.

Avoid scattered formulas such as:

* arbitrary `+ 0.5`
* arbitrary `- 0.5`
* map-size centering offsets
* duplicated `Floor`
* duplicated `Round`
* manual cell-to-world conversions

when those values represent authoritative map/grid semantics.

Use the appropriate Core conversion APIs.

---

## World Origin Convention

The authoritative convention is:

**World X/Z `(0,0)` is the center of logical map cell / terrain tile `(0,0)`.**

Client rendering must respect this convention.

Do not visually re-center an incorrectly interpreted logical map by applying an arbitrary root-node offset.

If a visual representation is offset while Core coordinates are correct, investigate the Client representation before changing Core mathematics.

---

## Visual Offsets and Pivots

Visual model origins are Client concerns.

If a mesh, scene, sprite, or model has a pivot that does not align with the authoritative world position:

* correct the scene
* correct the visual child transform
* correct the rendering adapter

Do not alter authoritative Core coordinates to compensate for a model pivot.

A visual offset and a logical coordinate offset are different problems.

Keep them separate.

---

## Terrain Rendering

Client terrain code renders authoritative terrain data from Core.

Client may own:

* terrain mesh generation
* terrain chunk views
* materials
* shaders
* render-node positioning
* collision representation where appropriate

Client must not regenerate independent authoritative terrain state.

Terrain views should derive from Core terrain data.

Do not duplicate procedural terrain generation in Client.

---

## Views

Views should remain focused on representing Core state.

Avoid allowing a single view class to accumulate unrelated responsibilities such as:

* data ownership
* gameplay rules
* mesh generation
* input handling
* orchestration
* simulation

When responsibilities become substantial and independently meaningful, separate them.

For example, mesh-building logic may belong in a dedicated mesh builder rather than inside a view that also owns node lifecycle and data binding.

Do not split classes merely to reduce line count; split when responsibilities are genuinely distinct.

---

## Managers

Client managers should primarily coordinate Client concerns.

Managers may:

* create views
* destroy views
* bind Core state to views
* coordinate Client flows
* route integration events
* manage Client-side resources

Avoid turning managers into universal service locators or containers for unrelated behavior.

A manager should have a coherent responsibility.

---

## Core-to-Client Synchronization

Client should react to authoritative Core changes through explicit APIs, events, or controlled synchronization mechanisms.

Do not maintain competing mutable copies of authoritative state unless there is a concrete presentation need.

If Client caches derived state, consider:

* who owns the cache
* how it is invalidated
* whether it can become stale
* whether rebuilding it is simpler

Prefer reconstructable derived Client state over duplicated authority.

---

## Events

Core events may notify Client that authoritative state changed.

Client event handlers may:

* spawn visuals
* update visuals
* remove visuals
* refresh UI
* trigger presentation effects

Client event handlers must not be required to complete the authoritative Core operation.

The Core state should already be logically correct when the event is emitted.

---

## UI

UI represents and controls interaction with Core state.

UI must not become authoritative game state.

Avoid placing gameplay rules inside:

* buttons
* panels
* HUD elements
* menus
* controls

UI may collect input and display Core results.

Domain decisions belong in Core.

---

## Camera

Camera behavior is entirely Client-side.

Camera position, smoothing, zoom, rotation, and transitions must not affect authoritative gameplay state unless an explicit game mechanic requires converting a player interaction into a Core intent.

---

## Client State

Some state legitimately belongs only in Client.

Examples include:

* currently open panel
* active preview object
* hover state
* camera position
* selection visuals
* temporary animation state
* cursor representation

Keep presentation-only state out of Core.

Do not promote transient UI or visual state into authoritative simulation state merely because it needs to persist for several frames.

---

## Initialization and Ownership

Initialization should reflect actual architectural ownership.

Do not force every Client object to be initialized directly by a single root manager merely for consistency.

Prefer clear ownership relationships.

A parent/coordinator should initialize a subsystem when that subsystem is genuinely part of its responsibility.

Avoid global initialization chains that obscure ownership.

---

## Error Handling

Differentiate between:

* Core validation failure
* missing visual representation
* invalid Client initialization
* broken Godot scene configuration
* violated programming invariant

Do not reinterpret a Core validation failure as a Client exception.

Likewise, do not hide broken Client configuration by changing authoritative Core behavior.

---

## Testing

Core is the primary unit-tested layer.

Client is intentionally less unit-testable because it integrates heavily with Godot.

Do not move logic into Client simply because it is difficult to test.

Instead, keep authoritative logic in Core where it can be tested deterministically.

Client testing may be added where useful, but Client architecture should favor:

* thin integration
* small adapters
* deterministic Core dependencies
* limited presentation-specific logic

Do not duplicate Core logic in Client solely to make a Client component independently testable.

---

## Performance

Rendering and UI performance matter, especially for large maps and many visual entities.

Before optimizing, consider:

* whether work occurs every frame
* unnecessary allocations
* mesh rebuild frequency
* node count
* draw-call impact
* repeated Core queries
* unnecessary visual updates
* event frequency

Prefer event-driven updates over continuous polling when state changes infrequently.

Do not sacrifice Core authority or architectural boundaries for premature Client optimization.

---

## Refactoring Convention

Before modifying a Client subsystem:

1. Inspect the implementation.
2. Inspect which Core APIs it consumes.
3. Inspect its callers and owning manager/flow.
4. Determine whether the behavior is presentation-specific or authoritative.
5. Preserve the existing dependency direction.
6. Avoid unrelated refactors.

If Client contains duplicated authoritative logic, prefer removing the duplication and routing through Core rather than expanding the Client implementation.

---

## Scope Discipline

Keep changes focused.

Avoid:

* unrelated scene restructuring
* broad renaming
* arbitrary hierarchy changes
* speculative manager abstractions
* unnecessary wrappers
* rewriting stable Godot integrations without a concrete reason

Do not redesign Client architecture merely because another Godot project might conventionally structure it differently.

Cosmos Casino intentionally separates Core simulation from Client representation.

---

## Agent Expectations

When working in Client:

* Treat Core as authoritative.
* Keep gameplay rules out of Client.
* Preserve `Client -> Core` dependency direction.
* Keep Godot-specific concerns inside Client.
* Use authoritative Core coordinate semantics.
* Avoid duplicate validation and coordinate math.
* Keep visual offsets separate from logical offsets.
* Prefer event-driven updates over unnecessary polling.
* Preserve centralized input arbitration.
* Inspect ownership before adding manager dependencies.
* Avoid unrelated refactors.
* Build the relevant project after implementation.
* Report important assumptions or architectural conflicts.

If a requested implementation would require making Client authoritative, identify the architectural conflict instead of silently doing so.

---

## Guiding Principle

Client should answer:

**"How is the authoritative game represented and interacted with in Godot?"**

Core should answer:

**"What is the authoritative state of the game, and what is allowed to happen?"**

Keep that boundary explicit.
