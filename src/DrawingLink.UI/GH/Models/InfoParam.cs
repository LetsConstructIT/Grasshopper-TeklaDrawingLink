using System;

namespace DrawingLink.UI.GH.Models
{
    public class InfoParam : Param
    {
        public string Value { get; }

        public InfoParam(string value, float top) : base(string.Empty, top)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
