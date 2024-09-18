using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    public class ModifyViewComponent : TeklaComponentBaseNew<ModifyViewCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ModifyView;
        public ModifyViewComponent() : base(ComponentInfos.ModifyViewComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (views, attributes, names, viewCss, displayCss, restrictionBoxes) = _command.GetInputValues();

            for (int i = 0; i < views.Count; i++)
            {
                var attribute = attributes.HasItems() ? attributes.ElementAtOrLast(i) : null;
                var name = names.HasItems() ? names.ElementAtOrLast(i) : null;
                var viewCs = viewCss.HasItems() ? viewCss.ElementAtOrLast(i) : null;
                var displayCs = displayCss.HasItems() ? displayCss.ElementAtOrLast(i) : null;
                var restrictionBox = restrictionBoxes.HasItems() ? restrictionBoxes.ElementAtOrLast(i) : null;

                ApplyAttributes(views[i], attribute, name, viewCs, displayCs, restrictionBox);
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, views);
        }

        private void ApplyAttributes(View view, string? attributes, string? name, GH_Plane viewCs, GH_Plane displayCs, GH_Box restrictionBox)
        {
            view.Select();

            if (attributes != null)
                view.Attributes.LoadAttributes(attributes);

            if (name != null)
                view.Name = name;

            if (viewCs != null && viewCs.Value != null)
                view.ViewCoordinateSystem = viewCs.Value.ToTekla();

            if (displayCs != null && displayCs.Value != null)
                view.DisplayCoordinateSystem = displayCs.Value.ToTekla();

            if (restrictionBox != null)
            {
                var xRange = restrictionBox.Value.X.ToTekla();
                var yRange = restrictionBox.Value.Y.ToTekla();
                var zRange = restrictionBox.Value.Z.ToTekla();

                view.RestrictionBox = new Tekla.Structures.Geometry3d.AABB(
                    new Tekla.Structures.Geometry3d.Point(xRange.Min, yRange.Min, zRange.Min),
                    new Tekla.Structures.Geometry3d.Point(xRange.Max, yRange.Max, zRange.Max));
            }

            view.Modify();
        }
    }

    public class ModifyViewCommand : CommandBase
    {
        private readonly InputListParam<View> _inViews = new InputListParam<View>(ParamInfos.View);
        private readonly InputOptionalListParam<string> _inAttributes = new InputOptionalListParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalListParam<string> _inNames = new InputOptionalListParam<string>(ParamInfos.Name);
        private readonly InputOptionalListParam<GH_Plane> _inViewCs = new InputOptionalListParam<GH_Plane>(ParamInfos.ViewCoordinateSystem);
        private readonly InputOptionalListParam<GH_Plane> _inDisplayCs = new InputOptionalListParam<GH_Plane>(ParamInfos.DisplayCoordinateSystem);
        private readonly InputOptionalListParam<GH_Box> _inRestrictionBoxes = new InputOptionalListParam<GH_Box>(ParamInfos.ViewRestrictionBox);

        private readonly OutputListParam<View> _outViews = new OutputListParam<View>(ParamInfos.View);

        internal (List<View> views, List<string>? attributes, List<string>? names, List<GH_Plane> viewCs, List<GH_Plane> displayCs, List<GH_Box> restrictionBoxes) GetInputValues()
        {
            return (
                _inViews.Value,
                _inAttributes.GetValueFromUserOrNull(),
                _inNames.GetValueFromUserOrNull(),
                _inViewCs.GetValueFromUserOrNull(),
                _inDisplayCs.GetValueFromUserOrNull(),
                _inRestrictionBoxes.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<View> reinforcements)
        {
            _outViews.Value = reinforcements;

            return SetOutput(DA);
        }
    }
}
