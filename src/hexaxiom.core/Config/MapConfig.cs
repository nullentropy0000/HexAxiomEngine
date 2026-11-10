using System.Text.Json.Serialization;

namespace HexAxiom.Config;

public sealed record MapConfig
{
    [JsonPropertyName("grid")]
    public required GridSection Grid { get; init; }

    [JsonPropertyName("occupants")]
    public List<OccupantSection> Occupants { get; init; } = new();
}

public sealed record GridSection
{
    [JsonPropertyName("size")]
    public int Size { get; init; } = 4;
}

public sealed record OccupantSection
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("kind")]
    public string Kind { get; init; } = "Character";

    [JsonPropertyName("label")]
    public string? Label { get; init; }

    [JsonPropertyName("q")]
    public int Q { get; init; }

    [JsonPropertyName("r")]
    public int R { get; init; }
}
