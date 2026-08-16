# Cosmos Casino

A systemic single-player survival / tycoon / colony-sim built with Godot Engine and C#.

Cosmos Casino focuses on deterministic simulation, scalable architecture, and long-term maintainability over rapid prototyping shortcuts. The project is heavily inspired by emergent-system games such as RimWorld while pursuing its own simulation-first direction.

## Tech Stack

- **Engine:** Godot 4.7.1
- **Language:** C# / .NET 10
- **Architecture:** Layered Core + Client separation
- **Testing:** NUnit
- **Version Control:** Git + GitHub

## Architectural Philosophy

The project follows a strict separation between simulation and presentation.

### Core Layer

The Core layer is:

- Engine-agnostic
- Deterministic
- Fully unit-testable
- Authoritative for save/load state
- Responsible for simulation and validation

Core contains:

- Terrain generation
- Map logic
- Build logic
- Validation systems
- Intent execution
- Simulation state

The Core layer does not depend on Godot.

### Client Layer

The Client layer is Godot-specific and responsible for:

- Rendering
- Input
- Camera systems
- Mesh generation
- UI
- Preview systems
- Visual synchronization

Client objects are disposable visual representations of authoritative Core state.

The Client is non-authoritative and is not intended to be the primary unit-testing target.

## Development Notes

This repository is under active architectural iteration.

Breaking refactors are expected while foundational systems stabilize.

Many systems currently prioritize correctness, explicitness, and long-term scalability over short-term convenience or premature optimization.

## Contributions

This project is not accepting external contributions at this time.
