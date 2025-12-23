using System;

namespace DrawingLink.UI.GH.Models
{
    public abstract class Param
    {
        public string Name { get; }
        public float Top { get; }
        public TableColumnInfo TableColumnInfo { get; }

        protected Param(string name, float top, TableColumnInfo tableColumnInfo)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Top = top;
            TableColumnInfo = tableColumnInfo ?? throw new ArgumentNullException(nameof(tableColumnInfo));
        }
    }
}
