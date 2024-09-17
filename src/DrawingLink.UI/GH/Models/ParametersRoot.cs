using System.Collections.Generic;

namespace DrawingLink.UI.GH.Models
{
    public class ParametersRoot
    {
        private readonly List<UiTab> _tabs;
        public IReadOnlyList<UiTab> Tabs => _tabs;

        public ParametersRoot()
        {
            _tabs = new List<UiTab>();
        }

        public void AddTab(UiTab tab)
        {
            _tabs.Add(tab);
        }
    }
}
