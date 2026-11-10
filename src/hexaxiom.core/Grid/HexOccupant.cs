namespace HexAxiom.Grid;

public enum HexOccupantKind
{
    Empty, ///just free to move to
    Character,
    Object,

    //In the future we will have ground types, terrain features, and other things that can occupy a hex. For now, we will just have characters and objects.
}

public sealed record HexOccupant(string Id, HexOccupantKind Kind, string? Label = null);
