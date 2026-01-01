using System;

namespace DrawingLink.UI.GH.Models;
public class ImageStyle
{
    public bool IsBackground;

    public string PositionX;

    public string PositionY;

    public int PaddingX;

    public int PaddingY;

    public int Width;

    public int Height;

    public bool SizeTypePercent;

    public ImageStyle(bool isBackground, string positionX, string positionY, int paddingX, int paddingY, int width, int height, bool sizeTypePercent)
    {
        IsBackground = isBackground;
        PositionX = positionX ?? throw new ArgumentNullException(nameof(positionX));
        PositionY = positionY ?? throw new ArgumentNullException(nameof(positionY));
        PaddingX = paddingX;
        PaddingY = paddingY;
        Width = width;
        Height = height;
        SizeTypePercent = sizeTypePercent;
    }
}
