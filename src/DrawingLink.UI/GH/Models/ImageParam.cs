using System;
using System.Drawing;

namespace DrawingLink.UI.GH.Models
{
    public class ImageParam : Param
    {
        public Bitmap Value { get; }
        public ImageStyle ImageStyle { get; }

        public ImageParam(Bitmap value, float top, ImageStyle imageStyle, TableColumnInfo tableColumnInfo) : base(string.Empty, top, tableColumnInfo)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            ImageStyle = imageStyle ?? throw new ArgumentNullException(nameof(imageStyle));
        }
    }
}
