using Grasshopper.Kernel.Attributes;
using GTDrawingLink.Tools;

namespace GTDrawingLink.Components
{
    public class CreateDatabaseObjectComponentAttributesNew<T> : GH_ComponentAttributes where T : CommandBase, new()
    {
        public override bool Selected
        {
            get => base.Selected;
            set
            {
                if (value != Selected)
                {
                    var createModelObjectComponent = base.Owner as CreateDatabaseObjectComponentBaseNew<T>;
                    if (value)
                    {
                        createModelObjectComponent.HighlightObjects();
                    }
                    else
                    {
                        createModelObjectComponent.UnHighlightObjects();
                    }
                }
                base.Selected = value;
            }
        }

        public CreateDatabaseObjectComponentAttributesNew(CreateDatabaseObjectComponentBaseNew<T> owner)
            : base(owner)
        {
        }
    }
}
