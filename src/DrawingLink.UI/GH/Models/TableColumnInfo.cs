using System;

namespace DrawingLink.UI.GH.Models;

public class TableColumnInfo
{
    public Guid TableContainerId { get; }

    public Guid ColumnId { get; }

    public int ColumnNumber { get; }
    public int RowNumber { get; set; }

    public string? ColumnStyle { get; }

    public TableColumnInfo(Guid tableContainerId, Guid columnId, int columnNumber, string? columnStyle)
    {
        TableContainerId = tableContainerId;
        ColumnId = columnId;
        ColumnNumber = columnNumber;
        ColumnStyle = columnStyle;
        RowNumber = 0;
    }

    internal static TableColumnInfo Empty()
    {
        return new TableColumnInfo(Guid.Empty, Guid.Empty, -1, string.Empty);
    }

    public bool IsValidTable() => TableContainerId != Guid.Empty;
}
