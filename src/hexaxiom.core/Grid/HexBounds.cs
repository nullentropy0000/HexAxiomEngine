namespace HexAxiom.Grid;

public readonly record struct HexBounds(int MinQ, int MinR, int MaxQ, int MaxR)
{
    public static HexBounds Rectangle(int size) => new(0, 0, size - 1, size - 1);
}
