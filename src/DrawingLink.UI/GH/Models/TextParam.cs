using System;

namespace DrawingLink.UI.GH.Models
{
    public class TextParam : PersistableParam
    {
        public string Value { get; }

        public TextParam(string fieldName, string name, string value, float top, TableColumnInfo tableColumnInfo) : base(fieldName, name, top, tableColumnInfo)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
