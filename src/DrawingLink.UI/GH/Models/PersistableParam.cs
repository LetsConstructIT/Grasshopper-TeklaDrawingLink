using System;

namespace DrawingLink.UI.GH.Models
{
    public abstract class PersistableParam : Param
    {
        public string FieldName { get; }

        protected PersistableParam(string fieldName, string name, float top) : base(name, top)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }

        public bool HasValidFieldName()
            => !string.IsNullOrEmpty(FieldName);
    }
}
