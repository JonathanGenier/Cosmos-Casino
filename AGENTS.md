# Cosmos Casino — Agent Instructions

## Project

Cosmos Casino is a single-player survival, tycoon, and colony-simulation game built with:

* Godot 4.7.1
* C#
* .NET 8
* NUnit

The project prioritizes long-term architectural correctness, maintainability, explicit ownership, testability, and deterministic behavior over short-term implementation convenience.

Prefer architecture that will scale with the project rather than shortcuts that make one feature easier to implement.

---

# Solution Architecture

The solution is divided primarily into:

* `CosmosCasino.Core` — authoritative game simulation
* `CosmosCasino.Client` — Godot presentation and integration layer
* `CosmosCasino.Tests` — automated tests for Core

The primary dependency direction is:

`Client -> Core`

Core must never depend on Client.

Tests primarily target Core.

Each project may contain its own `AGENTS.md` with more specific rules.

When working in a nested project, follow both this file and the most specific applicable `AGENTS.md`.

---

# Core Authority

`CosmosCasino.Core` is the authoritative source of gameplay truth.

Core determines:

* What exists
* What state it is in
* What actions are valid
* What actions change state
* Simulation results
* Authoritative spatial and domain rules

Client represents and interacts with Core state but must not redefine authoritative behavior.

If Core state and Client representation disagree, Core is authoritative.

Do not change authoritative Core rules merely to compensate for:

* Visual offsets
* Scene pivots
* Rendering issues
* UI behavior
* Godot lifecycle quirks
* Client implementation convenience

Fix presentation problems in Client unless the Core model itself is incorrect.

---

# Dependency Boundaries

Preserve clear dependency direction and ownership.

Core must remain engine-independent.

Client may depend on Core.

Tests may depend on Core.

Avoid:

* Core depending on Godot
* Core depending on Client
* Circular dependencies between architectural layers
* Presentation concerns leaking into authoritative simulation
* Client implementations duplicating Core rules

When two systems need to interact, prefer an explicit integration boundary over convenience coupling.

---

# Architectural Ownership

Every important piece of state or behavior should have a clear owner.

Avoid:

* Multiple authoritative representations of the same state
* Duplicate validation
* Duplicate coordinate mathematics
* Managers accumulating unrelated domain rules
* Global mutable state without explicit ownership

Before adding behavior, determine which layer or domain logically owns it.

Prefer keeping rules near the state and information required to enforce them.

---

# Domain Modeling

Prefer explicit domain concepts when they prevent ambiguity or invalid usage.

Strongly typed domain values are intentional.

Do not replace meaningful domain types with generic primitives or vectors solely for convenience.

Two types may contain the same numeric values while representing different concepts or coordinate spaces.

Preserve those distinctions unless there is a genuine architectural reason to remove them.

---

# Coordinate Convention

Cosmos Casino uses a centered authoritative world coordinate system.

**World X/Z `(0,0)` is the center of logical map cell / terrain tile `(0,0)`.**

Logical world coordinates may be positive or negative.

Do not assume the logical map begins at `(0,0)` and extends only into positive coordinates.

Do not emulate world centering using arbitrary Client or root-node offsets.

Authoritative coordinate semantics and conversions belong in Core.

Local storage coordinates may remain zero-based where appropriate.

Keep distinct concepts explicit, such as:

* World coordinates
* Map coordinates
* Terrain coordinates
* Chunk coordinates
* Local/index coordinates
* Rendering positions

Do not duplicate authoritative coordinate formulas across systems.

---

# Player Actions and State Mutation

Player interaction should conceptually flow through:

`Input -> Intent / Request -> Validation -> Execution -> Core state mutation -> Client update`

Client may initiate an action.

Core decides the authoritative result.

Avoid direct Client mutation of authoritative state.

Where intents are used, they should represent what the player wants to do rather than presentation-specific behavior.

---

# Validation

Gameplay validation belongs in Core.

Client may display validation results but must not independently reproduce authoritative gameplay rules.

Avoid validating the same gameplay rule in multiple architectural layers.

Expected gameplay failures should be represented as normal domain outcomes where appropriate.

Exceptions should generally represent programmer errors, invalid assumptions, or broken invariants rather than normal gameplay rejection.

---

# Event-Driven Architecture

Prefer explicit and event-driven communication when state changes.

Core events should notify consumers about authoritative changes that have already occurred.

Client event handlers may update:

* Visuals
* UI
* Previews
* Audio
* Animation

Core correctness must never depend on a Client subscriber being present.

Avoid continuous polling when a meaningful state-change event can express the same behavior more clearly.

---

# Godot Lifecycle

Godot lifecycle methods are integration mechanisms, not sources of authoritative gameplay truth.

Do not move simulation or domain behavior into `_Process` or `_PhysicsProcess` merely because those methods are convenient.

Game flows and authoritative systems should use explicit operations, events, intents, simulation steps, or other architecture-appropriate mechanisms.

---

# Scope Discipline

Keep changes focused on the requested problem.

Do not perform unrelated:

* Refactors
* Renaming
* Formatting changes
* Cleanup
* Architectural redesign
* Abstraction introduction
* File movement

unless required for correctness or explicitly requested.

Prefer small, reviewable diffs.

Do not use a focused task as an opportunity to redesign unrelated systems.

---

# Existing Architecture First

Before making a substantial change:

1. Inspect the relevant implementation.
2. Inspect its callers and consumers.
3. Inspect related domain types.
4. Inspect relevant tests.
5. Determine current ownership and dependency direction.
6. Understand whether unusual-looking behavior is intentional.

Do not replace deliberate Cosmos Casino architecture with generic framework or Godot patterns without understanding the existing design.

---

# Architectural Decisions

Do not silently invent major architectural conventions.

When a task exposes an important ambiguity:

1. Inspect existing code and tests for evidence of intended behavior.
2. Prefer established project conventions.
3. Preserve existing boundaries.
4. Identify the ambiguity when it cannot safely be inferred.

Do not make broad architectural decisions solely to complete a task faster.

---

# Code Quality

Prefer:

* Explicit intent
* Strong typing
* Clear ownership
* Cohesive responsibilities
* Predictable APIs
* Appropriate encapsulation
* Minimal duplication
* Readable C#
* Deterministic behavior where applicable

Avoid:

* God classes
* Hidden side effects
* Magic offsets
* Convenience coupling
* Duplicate authoritative logic
* Premature abstractions
* Speculative architecture
* Unnecessary wrappers

Follow existing repository naming, formatting, and organizational conventions.

Do not impose a different coding style unless explicitly requested.

---

# Testing

Core is intended to be highly testable.

New or changed authoritative Core behavior should normally have automated tests.

Bug fixes should include regression tests when practical.

Tests should protect behavior and invariants rather than unnecessarily coupling themselves to implementation details.

Do not weaken or blindly rewrite tests simply because new code does not satisfy them.

When a test fails, determine whether the cause is:

* A regression
* An intentional behavior change
* An outdated test
* An architectural conflict

before changing the expectation.

More detailed testing conventions belong in `code/Tests/AGENTS.md`.

---

# Performance

Performance matters because Cosmos Casino is intended to become a large systemic simulation with potentially large maps and populations.

However:

**Correctness and maintainability come before speculative optimization.**

Before optimizing, consider:

* Algorithmic complexity
* Hot-path frequency
* Allocation behavior
* Data locality
* Collection sizes
* Update frequency
* Deterministic implications

Avoid introducing unnecessary:

* Concurrency
* Pooling
* Caching
* Unsafe code
* Per-frame work
* Complexity

without a concrete architectural or measured performance reason.

Design systems so they can be optimized later without requiring major architectural rewrites.

---

# Refactoring

Refactors should preserve behavior unless the task explicitly changes behavior.

Before refactoring:

1. Understand the existing responsibility boundaries.
2. Inspect affected callers.
3. Inspect relevant tests.
4. Identify the invariant being preserved.
5. Keep the resulting diff focused.

Prefer removing duplicated logic over adding another abstraction around the duplication.

Do not refactor solely to reduce file size or class count.

Split responsibilities when they are genuinely distinct.

---

# Agent Workflow

For substantial tasks:

1. Understand the affected architecture before modifying code.
2. Read the applicable `AGENTS.md` instructions.
3. Inspect relevant implementation and tests.
4. Preserve ownership and dependency boundaries.
5. Keep changes scoped.
6. Build the affected projects.
7. Run relevant tests.
8. Inspect the final diff.
9. Report important assumptions, risks, and unresolved issues.

Do not claim successful completion while relevant builds or tests are failing.

If a requested implementation conflicts with established architectural rules, report the conflict rather than silently violating the architecture.

---

# Guiding Principle

Cosmos Casino is intended to grow into a large, systemic simulation game.

Prefer:

**a slightly more explicit implementation that preserves a coherent architecture**

over:

**a convenient shortcut that weakens ownership, testability, determinism, or architectural boundaries.**

Changes should make the codebase easier to extend and reason about over the long term, not merely easier to complete today.
