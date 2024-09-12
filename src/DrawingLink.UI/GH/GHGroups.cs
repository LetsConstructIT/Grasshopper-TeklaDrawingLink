using System;
using System.Collections.Generic;
using System.Linq;

namespace DrawingLink.UI.GH
{
    public class GHGroups
    {
        public Dictionary<string, HashSet<Guid>> Tabs { get; }
        public Dictionary<string, HashSet<Guid>> Groups { get; }

        private readonly HashSet<Guid> _hidden = new();

        public GHGroups()
        {
            Tabs = new Dictionary<string, HashSet<Guid>>();
            Groups = new Dictionary<string, HashSet<Guid>>();
        }

        public void AddHidden(IEnumerable<Guid> guids)
        {
            foreach (var guid in guids)
                _hidden.Add(guid);
        }

        public bool HasHidden()
            => _hidden.Any();

        public bool ContainsHidden(Guid guid)
            => _hidden.Contains(guid);
    }
}
