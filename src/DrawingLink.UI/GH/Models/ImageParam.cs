using System;
using System.Drawing;

namespace DrawingLink.UI.GH.Models
{
    public class ImageParam : Param
    {
        public Bitmap Value { get; }

        public ImageParam(Bitmap value, float top) : base(string.Empty, top)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
