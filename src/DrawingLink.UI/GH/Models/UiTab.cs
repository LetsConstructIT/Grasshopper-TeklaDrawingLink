using System.Collections.Generic;

namespace DrawingLink.UI.GH.Models
{
    public class UiTab
    {
        private readonly List<UiGroup> _groups;
        public IReadOnlyList<UiGroup> Groups => _groups;

        public string Name { get; private set; }

        public UiTab(string name)
        {
            _groups = new List<UiGroup>();
            Name = name;
        }

        public void AddGroup(UiGroup tab)
        {
            _groups.Add(tab);
        }

        public void ChangeName(string name)
        {
            Name = name;
        }
    }
}
