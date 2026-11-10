# Math

## Objective
Pure arithmetic on hex coordinates: distance, neighbor offsets, coordinate-system conversions. No game state, no IO, no clock.

## Responsibilities
- `HexMath.Offset(direction)` returns the cube delta for each of the six directions.
- `HexMath.Step(coord, direction)` applies the offset and returns the resulting coordinate.
- `HexMath.Distance(a, b)` returns cube-distance between two coordinates.
- `HexMath.Neighbors(coord)` enumerates the six adjacent coordinates.
- Cube ↔ axial ↔ offset conversions, when needed by the renderer or rule code, live here and nowhere else.

## Out of scope
- The Math module never touches `Character`, `Action`, `Engine`, or `Grid` types beyond reading a `HexCoord` field. It is the lowest layer; everything else depends downward on it, never the reverse.

## Public types
- `HexMath` (static)
