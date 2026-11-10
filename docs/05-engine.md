# Engine

## Objective
Compose Time, Characters, and Rules into a single tick. The Engine is the only module allowed to reference all three.

## Responsibilities
- Advance the clock on each `Step()`.
- For each registered character, route any queued `Action` to its resolver; otherwise ask the character to `DeclareAction`.
- Apply the `Resolution.NewState` returned by the resolver back onto the character.
- Append every `GameEvent` from every resolver to the `EventLog`, then return the events via `DrainEvents`.
- Expose `Register`, `Enqueue`, `Step`, `DrainEvents` so heads (future visualization, harness REPL, networked clients) can drive or observe the simulation without coupling to internals.

## Out of scope
- The Engine has no game knowledge of its own. It does not know what an "action" is except as a key into the rule registry. Adding a new action means adding a resolver — never editing the Engine.

## Public types
- `IEngine`
- `Engine`
- `GameEvent` (sealed record hierarchy: `MovementEvent`, `TurnEndedEvent`, `RoundEndedEvent`, ...)
- `EventLog`
