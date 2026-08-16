# Cosmos Casino — Development Standards

This document defines implementation standards for Cosmos Casino.

Apply only the sections relevant to the code being changed.
Do not perform unrelated refactoring solely to satisfy this document.

> Public visibility is a promise.  
> Minimize exposure.  
> Be intentional.

---

# Core

## Type Design and Visibility

- [ ] Classes are `sealed` unless inheritance is explicitly intended.
- [ ] Core types are `internal` unless intentionally exposed as part of the Core API.
- [ ] Nested types use the most restrictive visibility appropriate for their responsibility.
- [ ] A type is made `public` only when it is intentionally part of the Core API.

## Partial Classes

- [ ] `partial` is used only for meaningful responsibility separation.
- [ ] Partial file names clearly describe their responsibility.
- [ ] Examples include `.Api.cs`, `.Commands.cs`, `.Logging.cs`, and `.State.cs`.
- [ ] Do not split a class into partial files solely to reduce file length.

## Core API Discipline

- [ ] Public API partial files end with `.Api.cs`.
- [ ] Public Core types expose only intentional API.
- [ ] `.Api.cs` files contain only public members.
- [ ] Public members do not exist outside the corresponding `.Api.cs` file.
- [ ] Internal implementation details remain outside `.Api.cs`.
- [ ] Public exposure is not added merely for convenience.

## Structure and Readability

- [ ] Regions follow this order when applicable:

  FIELDS
  CONSTRUCTORS / INITIALIZATION
  EVENTS
  PROPERTIES
  METHODS

- [ ] No public fields.
- [ ] Naming reflects intent and domain meaning rather than implementation mechanics.
- [ ] Regions improve navigation and do not hide excessive class responsibility.

## Lifecycle and Ownership

- [ ] Identify ownership before adding cleanup behavior.
- [ ] Only owners clean up resources they own.
- [ ] Cleanup exists only when there is an actual lifecycle requirement.
- [ ] Do not add speculative `Shutdown`, `Dispose`, or similar methods.
- [ ] Do not introduce cleanup APIs merely because they may be useful later.

---

# Client

## Type Design and Visibility

- [ ] Classes are `sealed` unless inheritance is explicitly intended.
- [ ] Visibility is kept as restrictive as practical.
- [ ] Top-level Client types are `internal` unless they need to be `public`.
- [ ] Nested types use the most restrictive visibility appropriate for their responsibility.
- [ ] Public visibility is used only when required by Godot integration or an intentional Client API.

## Godot Integration

- [ ] Visibility remains pragmatic where Godot requires access to a type or member.
- [ ] Godot integration requirements do not justify exposing unrelated implementation details.
- [ ] Godot lifecycle methods remain integration hooks rather than sources of authoritative gameplay state.

## Core Boundary

- [ ] Client does not become authoritative for gameplay state or rules.
- [ ] No unnecessary Core concepts are duplicated or reimplemented in Client.
- [ ] Authoritative coordinate, validation, simulation, and domain rules remain in Core.
- [ ] Client uses Core APIs rather than recreating Core behavior for convenience.

## Structure and Readability

- [ ] Regions follow this order when applicable:

  FIELDS
  CONSTRUCTORS / INITIALIZATION
  EVENTS
  PROPERTIES
  METHODS

- [ ] No public fields.
- [ ] Naming reflects intent rather than implementation mechanics.
- [ ] Client classes maintain a cohesive presentation or integration responsibility.

## Lifecycle and Ownership

- [ ] Identify ownership before adding cleanup behavior.
- [ ] Only owners clean up objects, nodes, subscriptions, or resources they own.
- [ ] Cleanup occurs at the actual lifecycle boundary where it is required.
- [ ] Do not add speculative `Shutdown`, `Dispose`, or `_ExitTree` methods.
- [ ] Do not add lifecycle methods merely because cleanup may theoretically be needed later.

---

# Tests

## Type and Method Visibility

- [ ] Test classes are `sealed`.
- [ ] Test classes are `internal`.
- [ ] Test methods are `public`.

## Structure and Readability

- [ ] Regions follow this order when applicable:

  FIELDS
  SETUP & TEARDOWN
  TEST CATEGORY
  HELPERS

- [ ] Tests are grouped by behavior or feature.
- [ ] Test names clearly communicate the behavior or invariant being verified.
- [ ] Helpers remain private unless there is a concrete reason otherwise.

## Lifecycle and Ownership

- [ ] Test setup owns only the resources it creates.
- [ ] Cleanup exists only when the test actually owns something requiring cleanup.
- [ ] Do not add speculative `Shutdown`, `Dispose`, `_ExitTree`, or teardown behavior.

---

# Final Review

Before considering a change complete:

- [ ] Review the applicable Core, Client, and/or Tests sections.
- [ ] Verify newly introduced public API is intentional.
- [ ] Verify ownership and lifecycle responsibilities are explicit.
- [ ] Verify no unnecessary visibility expansion was introduced.
- [ ] Verify no speculative cleanup behavior was introduced.
- [ ] Verify naming and structure reflect architectural intent.
- [ ] Verify the final diff contains no unrelated refactors or file movement.
- [ ] Build the affected projects.
- [ ] Run the relevant tests.
