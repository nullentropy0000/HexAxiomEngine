# Time

## Objective
Own the progress of the simulation clock — what round we are in, what turn within the round, and when those values change. Nothing else.

## Responsibilities
- Expose the current `Round` and `Turn` as queryable values.
- Provide a single verb (`Advance`) the Engine calls once per step.
- Emit a notification (`TurnAdvanced`) so the Engine can react without polling.
- Stay trivially testable: a `ManualClock` with no automatic time source.

## Out of scope
- The clock never reads or writes `CharacterState` and never knows what an "action" is. Time is a pure counter.
- Initiative order is not a clock concern. Order is a property of how the Engine walks the character list.

## Public types
- `IClock`
- `ManualClock`
