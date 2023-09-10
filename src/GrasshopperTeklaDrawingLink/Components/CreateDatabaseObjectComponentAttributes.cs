using Grasshopper.Kernel.Attributes;

namespace GTDrawingLink.Components
{
    public class CreateDatabaseObjectComponentAttributes : GH_ComponentAttributes
    {
        public override bool Selected
        {
            get => base.Selected;
            set
            {
                if (value != Selected)
                {
                    var createModelObjectComponent = base.Owner as CreateDatabaseObjectComponentBase;
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

        public CreateDatabaseObjectComponentAttributes(CreateDatabaseObjectComponentBase owner)
            : base(owner)
        {
        }
    }
}
