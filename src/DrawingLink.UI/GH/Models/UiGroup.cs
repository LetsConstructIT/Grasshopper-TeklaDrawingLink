using System.Collections.Generic;

namespace DrawingLink.UI.GH.Models
{
    public class UiGroup
    {
        private readonly List<Param> _params;
        public IReadOnlyList<Param> Params => _params;

        public string Name { get; }

        public UiGroup(string name)
        {
            _params = new List<Param>();
            Name = name;
        }

        public void AddParam(Param param)
        {
            _params.Add(param);
        }
    }
}
