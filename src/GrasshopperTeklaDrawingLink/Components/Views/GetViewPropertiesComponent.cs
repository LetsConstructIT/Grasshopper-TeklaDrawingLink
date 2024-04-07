using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Views
{
    public class GetViewPropertiesComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.ViewProperties;

        public GetViewPropertiesComponent() : base(ComponentInfos.GetViewPropertiesComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.ViewType, GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "N", "Name of the provided view", GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.ViewCoordinateSystem, GH_ParamAccess.item);
            AddPlaneParameter(pManager, ParamInfos.DisplayCoordinateSystem, GH_ParamAccess.item);
            AddBoxParameter(pManager, ParamInfos.ViewRestrictionBox, GH_ParamAccess.item);
            AddNumberParameter(pManager, ParamInfos.Scale, GH_ParamAccess.item);
            AddTextParameter(pManager, ParamInfos.ViewTags, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided View is null");
                return;
            }

            view.Select();

            DA.SetData(ParamInfos.ViewType.Name, view.ViewType.ToString());
            DA.SetData("Name", view.Name);
            DA.SetData(ParamInfos.ViewCoordinateSystem.Name, view.ViewCoordinateSystem.ToRhino());
            DA.SetData(ParamInfos.DisplayCoordinateSystem.Name, view.DisplayCoordinateSystem.ToRhino());
            DA.SetData(ParamInfos.ViewRestrictionBox.Name, GetRestrictionBox(view));
            DA.SetData(ParamInfos.Scale.Name, view.Attributes.Scale);
            DA.SetDataList(ParamInfos.ViewTags.Name, FormatTagsAsList(view.Attributes.TagsAttributes));
        }

        private List<string> FormatTagsAsList(View.ViewMarkTagsAttributes tagsAttributes)
        {
            var tags = new List<string>
            {
                EscapeCurlyBraces(tagsAttributes.TagA1.TagContent.GetUnformattedString()),
                EscapeCurlyBraces(tagsAttributes.TagA2.TagContent.GetUnformattedString()),
                EscapeCurlyBraces(tagsAttributes.TagA3.TagContent.GetUnformattedString()),
                EscapeCurlyBraces(tagsAttributes.TagA4.TagContent.GetUnformattedString()),
                EscapeCurlyBraces(tagsAttributes.TagA5.TagContent.GetUnformattedString()),
            };

            return tags;

            string EscapeCurlyBraces(string text)
            {
                return text.TrimStart("{\n").TrimEnd("}\n");
            }
        }

        private GH_Box GetRestrictionBox(View view)
        {
            var box = new Rhino.Geometry.Box(
                view.ViewCoordinateSystem.ToRhino(),
                view.RestrictionBox.ToRhino());

            return new GH_Box(box);
        }
    }
}
