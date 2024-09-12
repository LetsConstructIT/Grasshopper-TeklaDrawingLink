using System;
using System.Collections.Generic;
using System.Linq;

namespace DrawingLink.UI.GH
{
    public class GHGroups
    {
        private readonly Dictionary<string, HashSet<Guid>> _tabs = new();
        private readonly Dictionary<string, HashSet<Guid>> _groups = new();
        private readonly HashSet<Guid> _hidden = new();

        public bool WithoutTabsAndGroups()
            => !_tabs.Any() && !_groups.Any();

        public string? FindTabName(Guid guid)
            => _tabs.FirstOrDefault(g => g.Value.Contains(guid)).Key;

        public string? FindGroupName(Guid guid)
            => _groups.FirstOrDefault(g => g.Value.Contains(guid)).Key;

        public void AddGroup(IEnumerable<Guid> guids, string name)
        {
            if (!guids.Any())
                return;

            if (!_groups.ContainsKey(name))
                _groups[name] = new HashSet<Guid>();

            _groups[name].UnionWith(guids);
        }

        public void AddTab(IEnumerable<Guid> guids, string name)
        {
            if (!guids.Any())
                return;

            if (!_tabs.ContainsKey(name))
                _tabs[name] = new HashSet<Guid>();

            _tabs[name].UnionWith(guids);
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
