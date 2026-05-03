using System;

namespace DrawingLink.UI.StartupUtils;


[Flags]
public enum AppOperationMode
{
    UI = 1 << 0,
    Background = 1 << 1,
    Server = 1 << 2,
    Client = 1 << 3,
}
