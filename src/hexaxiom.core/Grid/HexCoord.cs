namespace HexAxiom.Grid;

public readonly record struct HexCoord(int Q, int R, int S)
{
    public bool IsValid => Q + R + S == 0;

    public static HexCoord FromAxial(int q, int r) => new(q, r, -q - r);

    public static HexCoord Origin => new(0, 0, 0);

    public override string ToString() => $"({Q}, {R}, {S})";
}
