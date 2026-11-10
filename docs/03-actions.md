# Actions

## Objective
Define the language of intent that flows from Characters to Rules. An `Action` (formerly `MoveCommand`) is a value object — nothing more than a structured package the Rules subsystem can pattern-match on.

## Responsibilities
- Enumerate every legal kind of action (`ActionKind`): `Pass`, `Move`, `Act`, `Attack`, `MoveThrough`.
- Carry the actor's id plus any parameters the resolver will need (e.g., target, direction, or spell ID).
- Be immutable and trivially constructible so both the REPL and a future network head can emit them with the same shape.

## Out of scope
- Actions have no behavior. They do not know about the grid, character state, or rules. They are pure data shaped for transport.

## Public types
- `Action`
- `ActionKind`
