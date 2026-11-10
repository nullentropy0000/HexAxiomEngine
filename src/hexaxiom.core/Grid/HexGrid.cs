using HexAxiom.Math;

namespace HexAxiom.Grid;

public sealed class HexGrid
{
    public HexBounds Bounds { get; }

    private readonly HashSet<HexCoord> _cells;
    private readonly Dictionary<HexCoord, HexOccupant> _occupantsByCell = new();
    private readonly Dictionary<string, HexCoord> _cellsByOccupant = new(StringComparer.Ordinal);

    public HexGrid(HexBounds bounds)
    {
        Bounds = bounds;
        _cells = EnumerateBoundsCells(bounds).ToHashSet();
        if (_cells.Count == 0)
        {
            throw new ArgumentException(
                $"Bounds {bounds} produced no valid cube cells (q+r+s=0).", nameof(bounds));
        }
    }

    public IReadOnlyCollection<HexCoord> Cells => _cells;

    public IReadOnlyDictionary<HexCoord, HexOccupant> Occupants => _occupantsByCell;

    public bool Contains(HexCoord coord) => _cells.Contains(coord);

    public bool TryStep(HexCoord from, HexDirection direction, out HexCoord next)
    {
        if (!from.IsValid)
        {
            throw new ArgumentOutOfRangeException(nameof(from), from, "from must satisfy q + r + s = 0.");
        }
        next = HexMath.Step(from, direction);
        return Contains(next);
    }

    public bool IsOccupied(HexCoord coord) => _occupantsByCell.ContainsKey(coord);

    public HexOccupant? GetOccupant(HexCoord coord)
        => _occupantsByCell.TryGetValue(coord, out var occupant) ? occupant : null;

    public HexCoord? GetCoordinateOf(string occupantId)
        => _cellsByOccupant.TryGetValue(occupantId, out var coord) ? coord : null;

    public bool TryPlace(HexCoord coord, HexOccupant occupant)
    {
        ArgumentNullException.ThrowIfNull(occupant);
        if (!Contains(coord)) return false;
        if (_occupantsByCell.ContainsKey(coord)) return false;
        if (_cellsByOccupant.ContainsKey(occupant.Id)) return false;
        _occupantsByCell[coord] = occupant;
        _cellsByOccupant[occupant.Id] = coord;
        return true;
    }

    private void Remove(HexCoord coord)
    {
        if (!_occupantsByCell.TryGetValue(coord, out var occupant)) return;
        _occupantsByCell.Remove(coord);
        _cellsByOccupant.Remove(occupant.Id);
    }

    private void Add(HexCoord coord, HexOccupant occupant)
    {
        _occupantsByCell[coord] = occupant;
        _cellsByOccupant[occupant.Id] = coord;
    }

    public bool TryMoveOccupant(string occupantId, HexCoord to)
    {
        if (!Contains(to)) return false;
        if (_occupantsByCell.ContainsKey(to)) return false;
        if (!_cellsByOccupant.TryGetValue(occupantId, out var from)) return false;
        
        var occupant = _occupantsByCell[from];
        Remove(from);
        Add(to, occupant);
        return true;
    }

    public bool TryRemoveOccupant(string occupantId)
    {
        if (!_cellsByOccupant.TryGetValue(occupantId, out var coord)) return false;
        Remove(coord);
        return true;
    }

    private static IEnumerable<HexCoord> EnumerateBoundsCells(HexBounds b)
    {
        for (var q = b.MinQ; q <= b.MaxQ; q++)
        {
            for (var r = b.MinR; r <= b.MaxR; r++)
            {
                var coord = HexCoord.FromAxial(q, r);
                if (coord.IsValid) yield return coord;
            }
        }
    }
}
