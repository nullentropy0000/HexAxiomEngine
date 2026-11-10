namespace HexAxiom.Harness;

internal sealed record CharacterConfig
{
    public required string Name { get; init; }
    public required int Body { get; init; }
    public required int Stun { get; init; }
    public required int Endurance { get; init; }
    public required int Attributes { get; init; } ///look up here for the attribute values
    public required int Skills { get; init; } ///look up here for the skill values

}

