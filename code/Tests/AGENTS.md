# Cosmos Casino Tests — Agent Instructions

## Architectural Role

`CosmosCasino.Tests` verifies the authoritative behavior of `CosmosCasino.Core`.

The test project exists to protect:

* Gameplay rules
* Domain invariants
* State transitions
* Validation behavior
* Coordinate mathematics
* Deterministic simulation behavior
* Regression-sensitive Core APIs
* Architectural assumptions that can be expressed through behavior

Tests should give confidence that Core remains correct as the implementation evolves.

---

## Primary Test Target

Tests primarily target:

`CosmosCasino.Core`

Core is intentionally designed to be highly unit-testable without Godot.

Tests should not require:

* Godot
* Scene trees
* Nodes
* Rendering
* UI
* Input devices
* Cameras
* Meshes
* Shaders
* Godot lifecycle methods

If authoritative behavior cannot be tested without Godot, first investigate whether that behavior belongs in Core.

---

## Framework

Tests use **NUnit**.

Follow the existing repository's NUnit conventions.

Prefer standard NUnit functionality unless there is a concrete need for an additional testing dependency.

Do not introduce mocking, assertion, fixture, or test-generation libraries merely for convenience.

---

## Testing Philosophy

Tests should verify **observable behavior and domain invariants**, not unnecessarily couple themselves to implementation details.

Prefer testing:

* Given this state
* When this operation occurs
* Then this authoritative result is produced

Avoid tests whose only purpose is verifying:

* private implementation structure
* internal call counts
* exact method sequencing
* arbitrary class decomposition
* incidental collection implementation
* refactoring-sensitive details with no domain significance

A valid internal refactor should not require rewriting large portions of the test suite when observable behavior remains unchanged.

---

## New Core Behavior Requires Tests

When authoritative Core behavior is added or changed, relevant tests should normally be added or updated in the same change.

This includes:

* New gameplay rules
* Validation changes
* Coordinate conversions
* State mutations
* Terrain behavior
* Map behavior
* Build behavior
* Intent execution
* Procedural generation behavior
* Bug fixes

A bug fix should generally include a regression test proving the bug cannot silently return.

Do not leave new Core behavior untested without a concrete reason.

---

## Test Structure

Prefer tests that clearly communicate:

1. Arrange the required authoritative state.
2. Perform one meaningful operation.
3. Assert the resulting behavior or invariant.

Keep setup focused on what the test actually requires.

Avoid excessive setup that hides what behavior is being tested.

Shared helpers and fixtures are appropriate when they improve clarity, but avoid constructing large test frameworks around simple domain behavior.

---

## Test Naming

Test names should clearly communicate:

* The behavior being exercised
* The relevant condition
* The expected result

Prefer names that describe behavior rather than implementation.

For example:

`PlaceFloor_WhenCellHasNoFloor_PlacesFloor`

is preferable to a vague name such as:

`TestPlaceFloor1`

Follow the existing repository naming convention when one is already established.

---

## One Logical Behavior Per Test

A test should usually verify one coherent behavior or invariant.

Multiple assertions are acceptable when they collectively describe the same logical result.

Do not split a single meaningful behavior into many tiny tests purely to enforce one assertion per test.

Likewise, do not create large tests that validate several unrelated behaviors at once.

---

## Determinism

Tests must be deterministic.

The same test should produce the same result every run.

Avoid dependencies on:

* Current date/time
* Frame timing
* Thread scheduling
* Execution order
* Machine configuration
* Random seeds generated at runtime
* External services
* Network access
* Godot lifecycle behavior

When randomness is involved, use explicit deterministic seeds.

A failing deterministic-generation test should be reproducible from its inputs.

---

## Coordinate and Spatial Tests

Coordinate mathematics is architecture-sensitive and must be tested thoroughly.

When modifying spatial logic, test:

* Zero
* Positive coordinates
* Negative coordinates
* Exact boundaries
* Values immediately before boundaries
* Values immediately after boundaries
* Chunk boundaries
* Map boundaries
* Conversion round trips where appropriate

The authoritative world convention is:

**World X/Z `(0,0)` is the center of logical cell/tile `(0,0)`.**

Tests involving coordinates should preserve this invariant.

Do not assume world coordinates begin at zero and only increase positively.

---

## Coordinate Types

Respect strong domain coordinate types in tests.

Do not bypass APIs using generic tuples or vectors merely to make tests shorter when the domain type itself is part of the invariant being verified.

Examples may include:

* `MapCellCoord`
* `TerrainTileWorldCoord`
* `TerrainTileLocalCoord`
* `TerrainChunkGridCoord`

Tests should help catch accidental mixing of coordinate spaces.

---

## Boundary Testing

Boundary behavior should be tested deliberately rather than incidentally.

Examples include:

* Empty vs occupied cells
* Map edge coordinates
* Chunk edge coordinates
* Minimum and maximum valid values
* Negative boundaries
* Transition between adjacent cells
* Validation immediately before and after a state change
* First and last entries of bounded collections

When a bug occurs at a boundary, add a regression test near that exact boundary.

---

## Validation Tests

For gameplay validation, test both:

* Operations that should succeed
* Operations that should fail

Where validation returns structured results, assert the meaningful domain result rather than merely checking a boolean when possible.

Expected validation failures are normal domain outcomes and should be tested as such.

---

## State Mutation Tests

When testing authoritative mutation:

1. Verify the initial state when relevant.
2. Execute the authoritative operation.
3. Verify the resulting state.
4. Verify important related invariants.

Do not test only a returned success value if the important behavior is the resulting Core state.

---

## No-Op Behavior

When an operation may intentionally result in no state change, test that behavior explicitly.

Examples may include:

* Attempting to place something already present
* Removing something absent
* Repeating an idempotent operation

Verify both the reported result and that authoritative state remains correct.

---

## Regression Tests

When fixing a bug:

1. Reproduce the bug with a failing test when practical.
2. Implement the fix.
3. Confirm the test now passes.
4. Keep the test as regression protection.

Regression tests should describe the underlying invariant rather than encode the accidental implementation that originally caused the bug.

---

## Procedural Generation

Procedural generation tests should use fixed seeds.

Test meaningful invariants such as:

* Same seed produces the same logical result
* Coordinate access is stable
* Bounds are respected
* Neighboring/chunk conversions remain correct
* Generated state satisfies required invariants

Avoid asserting huge snapshots of generated data unless the exact output itself is intentionally part of the contract.

Prefer focused deterministic assertions.

---

## Test Data Size

Use the smallest data set that adequately proves the behavior.

For systems such as terrain generation, tests should not instantiate the full playable map unless the test specifically requires full-scale behavior.

If production configuration is large, prefer APIs that allow tests to use smaller deterministic configurations.

Fast tests encourage frequent execution and reduce development friction.

---

## Isolation

Tests should not depend on the execution order of other tests.

Each test must construct or obtain the state it requires.

Avoid shared mutable state between tests.

Do not assume another test has:

* initialized a static value
* generated terrain
* created a map
* changed configuration
* reset global state

Tests should be independently runnable.

---

## Static and Global State

Be especially cautious when testing code involving static/global state.

Tests must not silently contaminate subsequent tests.

If global state is unavoidable:

* restore it reliably
* isolate its usage
* avoid parallel interference
* consider whether the production design itself should be improved

Do not disable test parallelism globally merely to hide unsafe shared state without investigating the cause.

---

## Mocks and Test Doubles

Prefer real Core domain objects when they are inexpensive and deterministic.

Use fakes, stubs, or mocks when they isolate a meaningful external dependency or make the behavior substantially clearer.

Do not mock every collaborator by default.

Excessive mocking couples tests to implementation structure and makes refactoring harder.

Interfaces intended as Core seams, such as deterministic providers, are appropriate places for test doubles.

---

## Exceptions

When an API should reject programmer misuse or impossible state through an exception, test:

* The expected exception type
* The relevant invalid input or state

Do not over-specify exact exception messages unless the message itself is part of the intended contract.

Expected gameplay validation failures should generally not be tested as exceptions unless the Core API intentionally models them that way.

---

## Collections

When testing collections, avoid depending on enumeration order unless order is part of the contract.

If order matters to gameplay or deterministic simulation, assert it explicitly.

If order does not matter, write assertions that do not accidentally make it part of the API.

---

## Floating-Point Values

Use appropriate tolerances when floating-point calculations genuinely require them.

Do not introduce tolerances unnecessarily for values that should be mathematically exact.

Coordinate logic based on integral cells should remain exact whenever possible.

If floating-point imprecision enters an authoritative calculation, determine whether the domain should instead use a more explicit representation.

---

## Performance Tests

Normal unit tests should not rely on strict elapsed-time assertions.

Machine-dependent timing tests are brittle.

For performance-sensitive Core behavior, prefer verifying:

* Algorithmic properties
* Bounded work
* Allocation-sensitive design where measurable and appropriate
* Correctness at representative scale

Dedicated benchmarks should be treated separately from normal behavioral unit tests.

---

## Test Helpers

Test helpers should improve readability.

They may:

* construct common domain state
* create deterministic fixtures
* reduce irrelevant setup
* expose meaningful test scenarios

Do not let test helpers become a second implementation of production logic.

A helper must not reproduce the algorithm being tested, because both implementations could contain the same mistake.

---

## Avoid Testing Through Client

When testing Core behavior, call Core directly.

Do not instantiate Client managers, Godot nodes, UI, or views merely to reach Core functionality.

Core tests should prove that authoritative behavior is correct independently of presentation.

---

## Refactoring Tests

When production code is refactored without changing behavior, prefer keeping existing behavioral tests intact.

Do not rewrite tests simply because:

* classes moved
* private methods changed
* implementation was decomposed
* internal data structures changed

If large numbers of tests break during an implementation-only refactor, inspect whether the tests are too coupled to internals.

---

## Existing Tests Are Evidence

Before changing existing behavior:

1. Read the relevant tests.
2. Understand what invariant they protect.
3. Compare that invariant with current architecture and task requirements.

Do not automatically change a failing test to match new code.

A failing test may indicate:

* A regression
* An intentional behavior change
* An outdated test
* An architectural conflict

Determine which one applies before modifying the expectation.

---

## Test Coverage

Coverage percentage is not the primary goal.

Prioritize meaningful coverage of:

* Important rules
* Complex logic
* Failure paths
* Boundaries
* State transitions
* Architectural invariants
* Previously observed regressions

Do not create low-value tests solely to increase a coverage metric.

---

## Agent Expectations

When modifying or adding Core behavior:

* Inspect relevant existing tests first.
* Add or update tests for authoritative behavior changes.
* Add regression tests for bug fixes.
* Keep tests deterministic.
* Use small representative test configurations.
* Include negative and boundary coordinates where relevant.
* Avoid Godot dependencies.
* Avoid implementation-detail coupling.
* Run the relevant tests after changes.
* Run the broader Core test suite when the change has cross-system impact.
* Investigate failures instead of blindly updating expected values.

When adding tests specifically:

* Understand the domain behavior before writing assertions.
* Test the intended invariant, not merely the current implementation.
* Avoid duplicating production algorithms inside the test.
* Keep the test clear enough that a future developer can understand why the behavior matters.

---

## Guiding Principle

Tests should answer:

**"Does Core still behave according to its authoritative rules and invariants?"**

They should not answer:

**"Is Core still implemented exactly the same way as when this test was written?"**

Tests exist to make architectural improvement safer, not to prevent it.
