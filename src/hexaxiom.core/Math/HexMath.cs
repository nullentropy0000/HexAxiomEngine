using HexAxiom.Grid;

namespace HexAxiom.Math;

public static class HexMath
{
    public static HexCoord Offset(HexDirection direction) => direction switch
    {
        HexDirection.East => new HexCoord(1, 0, -1),
        HexDirection.NorthEast => new HexCoord(1, -1, 0),
        HexDirection.NorthWest => new HexCoord(0, -1, 1),
        HexDirection.West => new HexCoord(-1, 0, 1),
        HexDirection.SouthWest => new HexCoord(-1, 1, 0),
        HexDirection.SouthEast => new HexCoord(0, 1, -1),
        _ => throw new ArgumentOutOfRangeException(nameof(direction)),
    };

    public static HexCoord Step(HexCoord from, HexDirection direction)
    {
        if (!from.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(from), from, "from must satisfy q + r + s = 0.");
        }
        var delta = Offset(direction);
        return new HexCoord(from.Q + delta.Q, from.R + delta.R, from.S + delta.S);
    }

    public static int Distance(HexCoord a, HexCoord b)
    {
        if (!a.IsValid || !b.IsValid)
        {
            throw new ArgumentException("Both coordinates must satisfy q + r + s = 0.");
        }
        return (System.Math.Abs(a.Q - b.Q) + System.Math.Abs(a.R - b.R) + System.Math.Abs(a.S - b.S)) / 2;
    }

    public static IEnumerable<HexCoord> Neighbors(HexCoord origin)
    {
        if (!origin.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(origin), origin, "origin must satisfy q + r + s = 0.");
        }
        foreach (HexDirection dir in Enum.GetValues<HexDirection>())
        {
            yield return Step(origin, dir);
        }
    }
}

