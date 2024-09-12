using System;

namespace DrawingLink.UI.GH.Models
{
    public class CatalogParamData : PersistableParam
    {
        public string Value { get; private set; }
        public Func<string, string> PickFromCatalog { get; }

        public CatalogParamData(string fieldName, string name, string value, Func<string, string> pickFromCatalog, float top) : base(fieldName, name, top)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            PickFromCatalog = pickFromCatalog ?? throw new ArgumentNullException(nameof(pickFromCatalog));
        }
    }
}
