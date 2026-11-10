# Harness

## Objective
The harness is a text-only debug/test head sitting above `IEngine`. It drives the Engine through REPL input and scripted replays, and renders the resulting state as ASCII for inspection. It is the canonical reference for a "head" consuming `IEngine` + `HexAsciiRenderer` — any future head (network bridge, GUI, save-game printer) follows the same shape.

## Responsibilities
- Two CLI entry modes: `repl` (default — interactive prompt) and `drain <path>` (script file, no prompt).
- Bootstrapping: load a JSON config (default `./default.json`, override with `--config <path>`), build `HexGrid`, materialize each `Character`, register them with the Engine.
- A single command vocabulary shared across both modes; identical tokenization and dispatch.
- Translate each command into one of:
  - `IEngine.Enqueue(...)` — for actions,
  - `IEngine.Step()` trigger,
  - a read-only query (`where`, `grid`).
- After every `Step`, call `IEngine.DrainEvents()`, print each event, then reprint the grid via `HexAsciiRenderer.Render`.
- Loop pacing (REPL only): `loop on` arms auto-stepping at the configured delay, `loop off` disarms it. Pacing source is set by `delay <ms>` — positive integers schedule steps on a timer, `0` means each auto-step waits for an Enter keypress.

## Command vocabulary
Shared between REPL and drain (`#`-prefixed lines are comments, drain mode only).

| verb     | args             | meaning                                                       |
| -------- | ---------------- | ------------------------------------------------------------- |
| `help`   | —                | list commands                                                 |
| `quit`   | —                | exit (REPL only; drain mode exits at EOF)                     |
| `grid`   | —                | reprint the current grid                                      |
| `where`  | `<id>`           | print position + stats of a character                         |
| `move`   | `<id> <dir>`     | enqueue a flat hex move (`ne`, `e`, `se`, `sw`, `w`, `nw`)    |
| `step`   | —                | run one `IEngine.Step()`                                      |
| `loop`   | `on` \| `off`    | arm / disarm auto-stepping                                    |
| `delay`  | `<ms>`           | ms between auto-steps; `0` ⇒ Enter-paced                     |

## Out of scope
- The harness must not bypass `IEngine`. No direct resolver calls, no clock manipulation, no neighbor math. The Engine's public surface is the entire contract.
- The harness must not own game state. It reads `IEngine.Characters` and `IEngine.Grid` purely for rendering.
- The harness does not implement rendering — it consumes `HexAsciiRenderer` from Core.
- The harness does not define game semantics. A "flat hex move" is the `Action(ActionKind.Move, direction)` shape only; the Rules subsystem decides what happens.

## Config schema (`default.json`)
Minimum. Anything else is punted for now.

| field                  | type   | meaning                                  |
| ---------------------- | ------ | ---------------------------------------- |
| `grid.radius`          | int    | bounding hex radius                      |
| `characters[].id`      | string | stable id matching `Action` actor        |
| `characters[].name`    | string | display name                             |
| `characters[].q, .r`   | int    | starting axial coords                    |
| `characters[].stats`   | object | `{ health, speed, defense }`             |
| `loop.delayMs`         | int    | ms between auto-steps; `0` ⇒ Enter-paced |

## Public types
- `Program` (top-level entry; arg parsing, mode dispatch, bootstrap)
- `ConsoleHarness` (REPL loop)
- `DrainRunner` (file-driven loop)
- `WorldConfig`, `GridConfig`, `CharacterConfig` (records)
- `ConfigLoader` (JSON, `System.Text.Json`)
