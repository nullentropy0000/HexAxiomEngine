# Rules

## Objective
Resolve an `Action` against current game state and return what happens. Rules are the only place where game semantics live.

## Responsibilities
- Define `IResolver` as a pure function shape: `Resolve(action, actor, grid) → (newState, events)`.
- Provide a registry (`IRuleSet`, `DefaultRuleSet`) that maps `ActionKind` to the right resolver.
- Ship a `MoveResolver` for the basic "flat hex move"; future rules plug in alongside it without engine changes.

## Out of scope
- Resolvers must not mutate state in place — they return a delta the Engine commits.
- Resolvers must not call the clock, the engine, or other resolvers. Composition is the Engine's job.
- Resolvers must not log directly to a console — they emit `GameEvent`s.

## Public types
- `IResolver`
- `IRuleSet`
- `DefaultRuleSet`
- `MoveResolver`
- `Resolution` (record returned by a resolver)
