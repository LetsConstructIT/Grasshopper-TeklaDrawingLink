﻿using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Drawings
{
    public class OpenDrawingComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.OpenDrawing;

        public OpenDrawingComponent() : base(ComponentInfos.OpenDrawingComponent)
        {

        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item);
            pManager.AddBooleanParameter("Show", "Show", "Whether to open the drawing as visible or in the background (faster). Visible by default", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<DatabaseObject>(ParamInfos.Drawing) as Drawing;
            if (drawing == null)
                return;

            if (!DrawingInteractor.CanDrawingBeOpened(drawing, out string message))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, message);
                return;
            }

            var show = true;
            DA.GetData("Show", ref show);

            if (drawing.UpToDateStatus == DrawingUpToDateStatus.PartsWereModified)
                DrawingInteractor.DrawingHandler.UpdateDrawing(drawing);

            DrawingInteractor.DrawingHandler.SetActiveDrawing(drawing, show);

            DA.SetData(ParamInfos.Drawing.Name, new TeklaDatabaseObjectGoo(drawing));
        }
    }
}
