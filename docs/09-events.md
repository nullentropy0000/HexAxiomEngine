# Events

## Objective
Define the event types the Engine emits and the interaction calculus that resolvers run to produce them. Events are the public record of *what happened*; resolvers are the only place where game logic lives. This doc pins both sides so resolvers stay pure and heads (harness, future UI) can render a faithful picture from event streams alone.

## Interaction kinds

Two classes of interaction exist:

- **Character ↔ Object** — anything occupying a hex that is not another character: tree, building, ground, "empty" hex (air over ground).
- **Character ↔ Character** — another character occupying a hex.

The two are deliberately split because their resolution math differs. Objects have destructible hit-points; characters have knockback / knockdown / stampede semantics. Both kinds are addressed through the same event hierarchy to keep heads uniform.

## The allowed-actions tree

On a character's turn, **before** any selection is made, the Engine computes the set of reachable interactions and exposes them as a tree:

```
root: (round, turn, character, position)
├── pass
├── move
│   └── <dir>: reachable iff current moveset permits
│              AND target is empty (air/ground) or actor is in a stampede state
├── act
│   └── <dir>: includes defend, heal, spell, etc. Triggers GetEligibleActions exploration.
├── attack
│   └── <dir>: manual targeting, triggers range/distance calculation
└── move-through
    └── <dir>: reachable iff target hex holds an Object or Character that blocks movement
```

The character (or the harness driving it) selects one leaf; that selection becomes an `Action(kind, parameters)`. The Engine **never** selects on the character's behalf — it only exposes the tree. Decisions live with the actor or the head that drives it.

## Effects and the calculation each one triggers

### Flat hex move
- **Trigger**: a `move` branch selected; target hex is empty (air/ground); the actor is **not** currently knocked-down.
- **Calculation**: none beyond bounding and occupancy. Character takes the target hex.
- **Events**: `MovementEvent(from, to)`.

### Knockback
- **Trigger**: an `attack` or `move-through` exceeds the target's resistance.
- **Calculation**:
  - `damage = excess_force / target.resistance`
  - `displacement = damage / target.weight_factor` (in hex steps, away from source)
- **Events**: `DamageEvent(target, source, damage)`, `DisplacementEvent(target, from, to, cause=Knockback)`.

### Knockdown
- **Trigger**: a knockback pushes the target's `knockdown_score` over its threshold, OR a `move-through` is performed against a standing character.
- **Calculation**: standard knockback, with `Condition.KnockedDown` applied to the target until cleared (e.g. at end of target's next turn).
- **Events**: same as knockback plus `ConditionApplied(target, KnockedDown)`.

### Move-through destruction
- **Trigger**: a `move-through` against an object or knocked-down character whose HP is below the actor's move-through damage output.
- **Calculation**:
  1. Deal move-through damage; if HP ≤ 0, the object is **destroyed**.
  2. **If destroyed**: the actor's remaining distance budget is recomputed *past* the destroyed hex using a separate rule — `DistanceAfterDestruction(actor, path)` — instead of the normal adjacency rule. This is how walls get broken and stampedes propagate.
- **Events**: `DamageEvent(object, actor, amount)`, `ObjectDestroyedEvent(object, cause=MoveThrough)`, followed by `MovementEvent(actor, pastObject)` covering the extended distance.

## Event hierarchy

All events are `GameEvent` records. Concrete types so far:

- `MovementEvent(round, turn, characterId, from, to)`
- `DamageEvent(round, turn, victimId, sourceId, amount)`
- `DisplacementEvent(round, turn, characterId, from, to, cause)`
- `ConditionApplied(round, turn, characterId, Condition)`
- `ConditionRemoved(round, turn, characterId, Condition)`
- `ObjectDestroyedEvent(round, turn, objectId, cause)`
- `TurnEndedEvent(round, turn)`
- `RoundEndedEvent(round)`

Where:
- `cause` on `DisplacementEvent` ∈ `{ Knockback, Knockdown, Stampede, ForcedMove }`.
- `cause` on `ObjectDestroyedEvent` ∈ `{ MoveThrough, Attack, ... }`.
- `Condition` is an enum: `KnockedDown`, with slots reserved for `Stunned`, `Burning`, etc. (extend on demand).

## Trigger contract
*"The Event class triggers a calculation"* is interpreted directionally:

- An event is the **output** of a resolver — never an input.
- Resolvers return `(newState, events[])` from `IResolver.Resolve(...)`.
- The Engine appends events to `EventLog` and drains them via `DrainEvents()`.
- A head (harness, future UI) reacts to events, never the reverse.

So when the user reads "the Event class triggers a calculation," the chain in code is: head sees an event → resolver repurposed/spawned → calculation runs → new events emitted. Heads drive; resolvers calculate; events record. The Engine stitches them together.

## Out of scope
- Events do not contain game logic. They are records of what happened.
- Events do not subscribe to other events. Composition lives in resolvers and the Engine.
- Events do not know about heads or rendering.
- Events do not know the Clock directly. Round/turn on an event is a snapshot the Engine stamps at emit time; resolvers read `TurnContext` if they need it.

## Public types
- `GameEvent` (abstract record base)
- The concrete event records listed under **Event hierarchy**.
- `Condition` (enum)
- `DisplacementCause` (enum)
- `DestructionCause` (enum)
