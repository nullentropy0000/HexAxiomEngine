# Characters

## Objective
Hold the data and identity of every entity in the simulation. A character is the union of immutable identity and an evolving state snapshot.

## Responsibilities
- Define `Character` (entity, holds current state) and `CharacterState` (immutable record: id, name, position, stats).
- Expose `DeclareAction(TurnContext)` — the character's only behavioral verb. It returns an `Action` describing *intent*; it never mutates state.
- Allow the Engine to apply a new `CharacterState` (returned by a resolver) via a single internal entry point.

## Out of scope
- Characters never apply their own actions. They declare; rules resolve.
- Characters never advance the clock and never observe other characters' states.
- Characters never subscribe to events. Game history lives at the Engine boundary.

## Public types
- `Character`
- `CharacterState`
- `Stats`
- `TurnContext`
