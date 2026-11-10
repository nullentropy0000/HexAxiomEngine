using System.Text;
using HexAxiom.Grid;

namespace HexAxiom.Render;

/// <summary>
/// Renders a <see cref="HexGrid"/> as ASCII text. Each hex occupies 2 character columns.
/// Rows are staggered: even rows start at column 0, odd rows start at column 1, ensuring
/// adjacent rows never share a column slot (per <c>docs/06-grid.md</c>).
/// </summary>
public sealed class HexAsciiRenderer
{
    private const int CellWidth = 2;
    private const int CellStride = CellWidth + 1;
    private const string EmptyMarker = "..";

    public string Render(HexGrid grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        var rowMaxQ = grid.Cells.Max(c => c.Q);
        var rows = grid.Cells
            .GroupBy(c => c.R)
            .OrderBy(g => g.Key)
            .Select(g => (R: g.Key, Cells: g.OrderBy(c => c.Q).ToList()))
            .ToList();

        var sb = new StringBuilder();
        foreach (var (r, cells) in rows)
        {
            AppendRow(sb, grid, r, cells, rowMaxQ);
            sb.AppendLine();
        }
        AppendCoordLegend(sb, grid);
        return sb.ToString();
    }

    private void AppendRow(StringBuilder sb, HexGrid grid, int r, IList<HexCoord> cells, int rowMaxQ)
    {
        var offset = (r & 1) == 1 ? 1 : 0;
        if (offset > 0) sb.Append(' ');

        var buffer = new char[offset + rowMaxQ * CellStride + CellWidth];
        Array.Fill(buffer, ' ');

        foreach (var coord in cells)
        {
            var col = offset + coord.Q * CellStride;
            var marker = RenderCell(grid, coord);
            buffer[col] = marker[0];
            if (marker.Length > 1) buffer[col + 1] = marker[1];
        }

        sb.Append(buffer);
    }

    private void AppendCoordLegend(StringBuilder sb, HexGrid grid)
    {
        sb.AppendLine();
        foreach (var (coord, occupant) in grid.Occupants)
        {
            var label = occupant.Label ?? occupant.Id;
            sb.Append("  ").Append(label).Append(" @ ").Append(coord).AppendLine();
        }
    }

    private static string RenderCell(HexGrid grid, HexCoord coord)
    {
        var occupant = grid.GetOccupant(coord);
        if (occupant is null) return EmptyMarker;
        if (!string.IsNullOrEmpty(occupant.Label))
        {
            return occupant.Label.Length >= 2
                ? occupant.Label[..2]
                : occupant.Label.PadRight(2);
        }
        return occupant.Kind switch
        {
            HexOccupantKind.Character => "C ",
            HexOccupantKind.Object => "O ",
            HexOccupantKind.Empty => EmptyMarker,
            _ => "??",
        };
    }
}
