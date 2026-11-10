# Resolution Pipeline

## Objective
Pin the full resolution hierarchy as an explicit pipeline. The Engine implements this pipeline in code; this doc is the contract for the order of operations, what each stage produces, and which subsystem owns each stage. Every per-subsystem doc hangs off this spine, so a contradiction here is a contract violation everywhere else.

```
Timer
  └─▶ Cycle            (initiative)
        └─▶ Turn(actor)        (whose turn)
              └─▶ Action(actor)      (what they do — Pass is a legal Action)
                    └─▶ Calculation      (Hit Calculation per resolver)
                          └─▶ Event          (immutable record)
                                └─▶ Result(target) (per-target delta)
                                      └─▶ Status             (Character ⨯ Object state replaced)
```

## Stages

### 1. Timer
- **Owns**: `Time` (`IClock`).
- **In**: nothing.
- **Out**: `round, turn` snapshot stamped onto `TurnContext`.
- **Behavior**: `Clock.Advance()` is the only step the Engine performs before asking any character to act. The Timer never observes characters, actions, or rules.

### 2. Cycle
- **Owns**: `Engine` (the initiative queue).
- **In**: registered `Character`s + any deferred initiative from prior `Pass`/`Move` actions.
- **Out**: the next `actor` for the turn — a `Character` reference plus its slot in the order.
- **Behavior**: pops the next actor from the initiative order. The Cycle knows **order only**, never state.

### 3. Turn(actor)
- **Owns**: `Engine`.
- **In**: the chosen `actor`, the current `TurnContext(round, turn, grid)`, and any queued `Action` for that actor (from a head).
- **Out**: an `Action` for the actor — see stage 4.
- **Behavior**: if a queued action exists for the actor, it becomes the Action; otherwise the actor is asked to `DeclareAction(ctx)` and the resulting `Action` is the Action. Nothing else happens during Turn.

### 4. Action(actor)
- **Owns**: `Character` (declaration) + `Actions` (the value-object shape).
- **In**: `TurnContext`, the actor's current state.
- **Out**: an `Action`.
- **Behavior**: legal Actions (extensible):
  - **`Pass`** — no Calculation runs. Actor's initiative returns to the Cycle queue (deferred). Events: `MovementEvent`-equivalent for initiative flow.
  - **`Move`** — flat hex move; no resolver needed. Emits `MovementEvent` directly.
  - **`Act`** — includes defend, heal, spell, etc. Triggers a tree exploration of possibilities (`GetEligibleActions`) to calculate targets.
  - **`Attack`** — directed at a target manually. Triggers max range calculation (e.g., guns require distance calculation).
  - **`MoveThrough`** — a moving attack (e.g., jumpkick). A class of motion that both attacks and moves.

### 5. Calculation
- **Owns**: `Rules` (`IResolver.Resolve`).
- **In**: the `Action`, the actor (`Character`), the grid, optional resolver-local state.
- **Out**: a `Resolution` — see stage 7.
- **Behavior**: a pure function. No clock reads, no event subscriptions, no head knowledge. A giant-sword Attack runs damage + radius + displacement math here and emits events; it does not touch characters or objects directly.

### 6. Event
- **Owns**: `Rules` (produced) + `Engine.EventLog` (collected) + heads (consumed).
- **In**: events returned by the Calculation.
- **Out**: events appended to `EventLog`; on `DrainEvents()`, fanned out to subscribers.
- **Behavior**: events are immutable records of *one* thing that happened ("3 damage to building X", "character Y was knocked down"). They do not know about other events, head implementations, or rendering. The Trigger contract from `09-events.md` applies here.

### 7. Result(target)
- **Owns**: `Rules` (returned) + `Engine` (applied).
- **In**: the `Resolution` from the Calculation.
- **Out**: per-target state updates applied to Characters ⨯ Objects.
- **Behavior**: a single Action may produce **multiple** per-target Results (cleave, AoE, stampede). `Resolution` is therefore a collection:
  ```
  Resolution
    ├── TargetResult[]            (one entry per affected target)
    │     ├── targetId     (CharacterId or ObjectId — discriminated union)
    │     ├── newState     (new CharacterState or ObjectState)
    │     └── events[]     (events this specific target caused)
    └── engineEvents[]      (events not bound to a single target)
  ```
  The Engine applies every `TargetResult` to its target. Rejected/unchanged targets still receive a `TargetResult` with `newState == oldState` so heads see them in the timeline.

### 8. Status
- **Owns**: `Characters` (per-character state) + `Objects` (per-object state) + `Engine` (orchestration).
- **In**: applied `TargetResult`s.
- **Out**: every Character and Object whose State was returned has its `State` replaced; heads read fresh state via `IEngine.Characters` and (future) `IEngine.Objects`.
- **Behavior**: status replacement is the **only** writer to Character/Object State outside of bootstrapping. Test for "did anything happen this step?" is `Events.Length > 0 || anyStateChanged`.

## Worked example — Giant swings a giant sword

A Giant's chosen Action is `Attack` aimed at a target hex. The Calculation (a Hit Calculus resolver) runs:

1. **Calculation** determines the cleave radius from the giant-sword perk.
2. For each target within radius the resolver returns a `TargetResult` with the appropriate events:
   - **Ground (Object)** — `events = [ObjectScuffed(ground, …)]`; State unchanged. One `TargetResult` emitted.
   - **Building (Object)** — HP reduced; if `HP ≤ 0`, `ObjectDestroyedEvent`. Two events on one `TargetResult`.
   - **Best-friend (Character)** — knockback or knockdown depending on resistance. `DamageEvent`, possibly `DisplacementEvent`, possibly `ConditionApplied(KnockedDown)`. One `TargetResult` carrying all three events.
3. The resolver returns **one** `Resolution` with three `TargetResult`s + engine-level events (e.g. `TurnEndedEvent`).

The Engine applies each `TargetResult` to the matching Character/Object. The harness head prints the events and re-renders the grid showing the giant-sword's aftermath.

## Out of scope
- The Engine does not know damage formulas or any other game math. All formulas live in specific resolvers (`AttackResolver`, `MoveThroughResolver`, ...).
- The pipeline is **synchronous**. A single Action's Calculation completes before another actor's Turn starts. No re-entry.
- The Cycle does not know about state — only turn order.
- Calculations do not call the Clock or other resolvers. Composition is the Engine's job.
- `Pass` does not consume the actor's turn for the round. The actor returns to the initiative queue.

## Public types introduced or implied

These flow from the pipeline; existing docs treat some of them, others are new:

- `Initiative` — Engine-side ordered queue of `CharacterId`s, including deferred entries from Pass/Delay.
- `Action` — the verb chosen by an actor.
- `ActionKind` — enum including at least `Pass`, `Move`, `Act`, `Attack`, `MoveThrough`.
- `Resolution` — already exists in `09-events.md`-adjacent code; now contains `TargetResult[]` plus engine events.
- `TargetResult(targetId, newState, events)` — the per-target delta shape.
- `TargetId` — discriminated union `{ CharacterId | ObjectId }`.
- `ObjectState`, `ObjectId` — **new**. Parallel to `CharacterState`. Lives in a doc not yet written (`11-objects.md` when we add it).
- `IIObjectStore` (Engine) — exposes `IEngine.Objects` for heads. **new**.
