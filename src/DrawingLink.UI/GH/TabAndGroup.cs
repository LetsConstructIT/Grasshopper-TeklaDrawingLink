using System;
using System.Diagnostics;

namespace DrawingLink.UI.GH
{
    [DebuggerDisplay("{Tab} - {Group}")]
    public class TabAndGroup
    {
        public string Tab { get; }
        public string Group { get; }

        public TabAndGroup(string tab, string group)
        {
            Tab = tab ?? string.Empty;
            Group = group ?? string.Empty;
        }
    }

    public class ObjectConnectivity
    {
        public int SourceCount { get; }
        public int RecipientCount { get; }

        public ObjectConnectivity(int sourceNo, int recipientNo)
        {
            SourceCount = sourceNo;
            RecipientCount = recipientNo;
        }

        public bool IsStandalone()
            => SourceCount == 0 && RecipientCount == 0;
    }
}
