using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;

namespace GTDrawingLink.Components.Miscs.Loops
{
    public class LoopKeeperComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.LoopKeeper;

        public LoopKeeperComponent() : base(ComponentInfos.LoopKeeperComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "Data to keep", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Swimlanes", "S", "Merge point for different flow paths needed for synchronization", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "Propagated data", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetDataTree(1, out GH_Structure<IGH_Goo> swimLanes))
                return;

            DA.GetDataTree(0, out GH_Structure<IGH_Goo> dataToBePropagated);
            DA.SetDataTree(0, dataToBePropagated);

            this.Message = $"Last update: {DateTime.Now:HH:mm:ss}";
        }
    }
}
