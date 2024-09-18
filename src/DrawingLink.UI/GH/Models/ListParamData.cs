using System.Collections.Generic;
using System.Linq;

namespace DrawingLink.UI.GH.Models
{
    public class ListParamData : PersistableParam
    {
        public IReadOnlyList<string> Items { get; }
        public string SelectedItem { get; }

        public ListParamData(string fieldName, string name, IEnumerable<string> items, string selectedItem, float top) : base(fieldName, name, top)
        {
            Items = items.ToArray();
            SelectedItem = selectedItem;
        }
    }
}
