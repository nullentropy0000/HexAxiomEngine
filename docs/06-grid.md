# Grid

## Objective
Model the geographic space the simulation inhabits. Coordinates are axial cube form `(q, r, s)` with `q + r + s = 0`.

## Responsibilities
- Define `HexCoord` (cube record struct with `IsValid` invariant) and `HexDirection` (six enum members).
- Define `HexGrid` as a finite bounded region of valid cells, answerable for `Contains` and an in-bounds `TryMove(from, dir)`.
- Provide `HexAsciiRenderer` to print the grid to text. Render with **staggered rows and two-character cells**, sidestepping the "square cell / double-occupancy" ambiguity by ensuring adjacent rows never claim the same column slot.

## Out of scope
- The grid does not know about characters or actions. Containment, bounds, and neighbor topology only. Arithmetic belongs in Math.

## Public types
- `HexCoord`
- `HexDirection`
- `HexGrid`
- `HexAsciiRenderer`
