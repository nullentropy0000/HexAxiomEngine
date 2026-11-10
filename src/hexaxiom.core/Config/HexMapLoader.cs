using System.Text.Json;
using HexAxiom.Grid;

namespace HexAxiom.Config;

public sealed record HexMapLoadResult(HexGrid Grid, IReadOnlyList<HexOccupant> PlacedOccupants);

public static class HexMapLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static HexMapLoadResult LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Map file not found: {path}", path);
        }
        using var stream = File.OpenRead(path);
        return LoadFromJson(stream);
    }

    public static HexMapLoadResult LoadFromJson(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        var config = JsonSerializer.Deserialize<MapConfig>(stream, JsonOptions)
            ?? throw new InvalidDataException("Empty or invalid map config.");
        return Build(config);
    }

    public static HexMapLoadResult LoadFromJsonString(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        var config = JsonSerializer.Deserialize<MapConfig>(json, JsonOptions)
            ?? throw new InvalidDataException("Empty or invalid map config.");
        return Build(config);
    }

    public static HexMapLoadResult Build(MapConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        var size = config.Grid.Size <= 0 ? 4 : config.Grid.Size;
        var grid = new HexGrid(HexBounds.Rectangle(size));
        var placed = new List<HexOccupant>(config.Occupants.Count);

        foreach (var spec in config.Occupants)
        {
            var occupant = new HexOccupant(
                Id: spec.Id,
                Kind: ParseKind(spec.Kind),
                Label: spec.Label);
            var coord = HexCoord.FromAxial(spec.Q, spec.R);
            if (!grid.TryPlace(coord, occupant))
            {
                throw new InvalidDataException(
                    $"Cannot place occupant '{spec.Id}' at ({spec.Q}, {spec.R}): " +
                    $"out of bounds or occupied.");
            }
            placed.Add(occupant);
        }

        return new HexMapLoadResult(grid, placed);
    }

    private static HexOccupantKind ParseKind(string raw) => raw?.ToLowerInvariant() switch
    {
        null => HexOccupantKind.Character,
        "character" or "char" => HexOccupantKind.Character,
        "object" or "obj" => HexOccupantKind.Object,
        _ => throw new InvalidDataException($"Unknown occupant kind '{raw}'."),
    };
}
