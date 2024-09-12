using Grasshopper.Kernel;
using System;

namespace DrawingLink.UI.GH
{
    public abstract class TeklaParamBase
    {
        public IGH_ActiveObject ActiveObject { get; }
        public string FieldName { get; set; }
        public bool IsMultiple { get; }
        public string Prompt { get; }

        public TeklaObjects TeklaObjects { get; protected set; }

        protected TeklaParamBase(IGH_ActiveObject activeObject, bool isMultiple, string prompt)
        {
            ActiveObject = activeObject ?? throw new ArgumentNullException(nameof(activeObject));
            IsMultiple = isMultiple;
            Prompt = prompt ?? throw new ArgumentNullException(nameof(prompt));
            FieldName = string.Empty;
        }
    }
}
