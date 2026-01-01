using DrawingLink.UI.GH.Models;
using Grasshopper.Kernel;
using System;

namespace DrawingLink.UI.GH
{
    public class ActiveObjectWrapper
    {
        public IGH_ActiveObject ActiveObject { get; }
        public TabAndGroup TabAndGroup { get; }
        public ObjectConnectivity Connectivity { get; }
        public TableColumnInfo TableColumnInfo { get; }
        public string FieldName { get; set; }

        public ActiveObjectWrapper(IGH_ActiveObject activeObject, TabAndGroup tabAndGroup, ObjectConnectivity connectivity, TableColumnInfo tableColumnInfo)
        {
            ActiveObject = activeObject ?? throw new ArgumentNullException(nameof(activeObject));
            TabAndGroup = tabAndGroup ?? throw new ArgumentNullException(nameof(tabAndGroup));
            Connectivity = connectivity ?? throw new ArgumentNullException(nameof(connectivity));
            TableColumnInfo = tableColumnInfo;
            FieldName = string.Empty;
        }
    }
}
