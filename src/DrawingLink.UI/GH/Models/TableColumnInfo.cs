using System;

namespace DrawingLink.UI.GH.Models;

public class TableColumnInfo
{
    public Guid TableContainerId;

    public Guid ColumnId;

    public int ColumnNumber;

    public string? ColumnStyle;

    public TableColumnInfo(Guid tableContainerId, Guid columnId, int columnNumber, string? columnStyle)
    {
        TableContainerId = tableContainerId;
        ColumnId = columnId;
        ColumnNumber = columnNumber;
        ColumnStyle = columnStyle;
    }

    internal static TableColumnInfo Empty()
    {
        return new TableColumnInfo(Guid.Empty, Guid.Empty, -1, string.Empty);
    }

    public bool IsValidTable() => TableContainerId != Guid.Empty;
}
