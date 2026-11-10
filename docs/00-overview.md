# Overview

HexAxiom.Core is a headless TTRPG simulation engine. The defining property is strict decoupling among three concerns that most engines entangle:

- **Time** scheduling is independent of any specific character.
- **Character** instantiation is independent of any specific rule.
- **Rules** application is independent of either of the above.

The **`Engine`** is the only module permitted to know all three. It composes them: it advances time, asks each character for an intent, hands that intent to the matching rules resolver, and applies the resulting state delta. Anything that observes game outcomes does so by subscribing to the event stream the Engine drains — the future visualization head is therefore not coupled to engine internals.

Docs in this folder:

| # | doc                              | role                                                    |
| - | -------------------------------- | ------------------------------------------------------- |
| 01 | [Time](./01-time.md)             | Pure clock. Subsystem synopsis.                         |
| 02 | [Characters](./02-characters.md) | Entity + state. Subsystem synopsis.                     |
| 03 | [Actions](./03-actions.md)         | Intent value vocabulary. Subsystem synopsis.           |
| 04 | [Rules](./04-rules.md)           | Resolver registry. Subsystem synopsis.                  |
| 05 | [Engine](./05-engine.md)         | Sole composer of the three. Subsystem synopsis.         |
| 06 | [Grid](./06-grid.md)             | Hex space + ASCII renderer. Subsystem synopsis.         |
| 07 | [Math](./07-math.md)             | Pure hex arithmetic. Subsystem synopsis.                |
| 08 | [Harness](./08-harness.md)       | Text-only debug head. Subsystem synopsis.               |
| 09 | [Events](./09-events.md)         | Cross-cutting spec — interaction kinds, effects, tree.  |
| 10 | [Pipeline](./10-resolution-pipeline.md) | The full resolution spine: Timer→…→Status.       |
