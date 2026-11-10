using System.Text.Json;
using HexAxiom.Config;
using HexAxiom.Grid;

namespace HexAxiom.Core.Tests;

public class HexMapLoaderTests
{
    private const string ValidConfig = """
    {
      "grid": { "size": 4 },
      "occupants": [
        { "id": "tree",   "kind": "object",    "q": 0, "r": 0, "label": "Tr" },
        { "id": "hero",   "kind": "character", "q": 2, "r": 0, "label": "He" },
        { "id": "rogue",  "kind": "char",      "q": 1, "r": 1, "label": "Ro" }
      ]
    }
    """;

    [Fact]
    public void LoadFromJsonString_PlacesAllOccupantsOnValidHexes()
    {
        var result = HexMapLoader.LoadFromJsonString(ValidConfig);

        Assert.Equal(3, result.PlacedOccupants.Count);
        Assert.Contains(result.Grid.Occupants, kv => kv.Value.Id == "hero" && kv.Key.Q == 2 && kv.Key.R == 0);
        Assert.Contains(result.Grid.Occupants, kv => kv.Value.Id == "rogue" && kv.Key.Q == 1 && kv.Key.R == 1);
        Assert.Contains(result.Grid.Occupants, kv => kv.Value.Id == "tree" && kv.Key.Q == 0 && kv.Key.R == 0);
        Assert.Equal(HexOccupantKind.Object, result.Grid.GetOccupant(HexCoord.FromAxial(0, 0))!.Kind);
        Assert.Equal(HexOccupantKind.Character, result.Grid.GetOccupant(HexCoord.FromAxial(2, 0))!.Kind);
    }

    [Fact]
    public void LoadFromJsonString_DefaultsKindToCharacterWhenOmitted()
    {
        const string json = """
        {
          "grid": { "size": 4 },
          "occupants": [
            { "id": "a", "q": 0, "r": 0 }
          ]
        }
        """;

        var result = HexMapLoader.LoadFromJsonString(json);

        var occupant = Assert.Single(result.PlacedOccupants);
        Assert.Equal(HexOccupantKind.Character, occupant.Kind);
    }

    [Fact]
    public void LoadFromJsonString_ThrowsWhenCharacterHexIsOccupiedByObject()
    {
        const string json = """
        {
          "grid": { "size": 4 },
          "occupants": [
            { "id": "boulder", "kind": "object",    "q": 0, "r": 0 },
            { "id": "hero",    "kind": "character", "q": 0, "r": 0 }
          ]
        }
        """;

        var ex = Assert.Throws<InvalidDataException>(() => HexMapLoader.LoadFromJsonString(json));
        Assert.Contains("hero", ex.Message);
        Assert.Contains("(0, 0)", ex.Message);
    }

    [Fact]
    public void LoadFromJsonString_ThrowsWhenFirstOccupantHexIsOutOfBounds()
    {
        const string json = """
        {
          "grid": { "size": 3 },
          "occupants": [
            { "id": "hero", "kind": "character", "q": 99, "r": 0 }
          ]
        }
        """;

        Assert.Throws<InvalidDataException>(() => HexMapLoader.LoadFromJsonString(json));
    }

    [Fact]
    public void LoadFromJsonString_ThrowsOnDuplicateOccupantId()
    {
        const string json = """
        {
          "grid": { "size": 4 },
          "occupants": [
            { "id": "hero", "kind": "character", "q": 0, "r": 0 },
            { "id": "hero", "kind": "character", "q": 1, "r": 0 }
          ]
        }
        """;

        Assert.Throws<InvalidDataException>(() => HexMapLoader.LoadFromJsonString(json));
    }

    [Fact]
    public void LoadFromJsonString_ThrowsOnUnknownOccupantKind()
    {
        const string json = """
        {
          "grid": { "size": 4 },
          "occupants": [
            { "id": "thing", "kind": "terrain", "q": 0, "r": 0 }
          ]
        }
        """;

        Assert.Throws<InvalidDataException>(() => HexMapLoader.LoadFromJsonString(json));
    }

    [Fact]
    public void LoadFromJsonString_RejectsBlankJson()
    {
        Assert.ThrowsAny<Exception>(() => HexMapLoader.LoadFromJsonString(""));
    }

    [Fact]
    public void LoadFromJsonString_RejectsMalformedJson()
    {
        Assert.Throws<JsonException>(() => HexMapLoader.LoadFromJsonString("{ this is not json"));
    }

    [Fact]
    public void LoadFromJsonString_AllowsEmptyOccupantList()
    {
        const string json = """{ "grid": { "size": 4 }, "occupants": [] }""";

        var result = HexMapLoader.LoadFromJsonString(json);

        Assert.Empty(result.PlacedOccupants);
        Assert.Equal(HexBounds.Rectangle(4), result.Grid.Bounds);
    }
}
