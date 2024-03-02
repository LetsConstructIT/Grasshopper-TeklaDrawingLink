using System;
using System.Collections.Generic;

namespace DrawingLink.UI.GH
{
    public class GHGroups
    {
        public Dictionary<string, HashSet<Guid>> Tabs = new();
        public Dictionary<string, HashSet<Guid>> Groups = new();
        public HashSet<Guid> Hidden = new();
    }
}
