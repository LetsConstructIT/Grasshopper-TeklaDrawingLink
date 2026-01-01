using System;

namespace DrawingLink.UI.GH.Models
{
    public class InfoParam : Param
    {
        public string Value { get; }

        public InfoParam(string value, float top, TableColumnInfo tableColumnInfo) : base(string.Empty, top, tableColumnInfo)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
