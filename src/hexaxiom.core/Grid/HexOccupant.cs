namespace HexAxiom.Grid;

public enum HexOccupantKind
{
    Empty,
    Character,
    Object,
}

public sealed record HexOccupant(string Id, HexOccupantKind Kind, string? Label = null);
