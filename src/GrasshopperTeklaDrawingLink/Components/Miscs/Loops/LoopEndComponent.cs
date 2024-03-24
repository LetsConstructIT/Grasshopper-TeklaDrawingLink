using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTDrawingLink.Components.Miscs.Loops
{
    public class LoopEndComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.LoopEnd;

        private LoopStartComponent _loopStart;
        private readonly GH_Structure<IGH_Goo> _mergedData = new GH_Structure<IGH_Goo>();

        public LoopEndComponent() : base(ComponentInfos.LoopEndComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new LinkParameter(ParamInfos.LoopEnd, GH_ParamAccess.item));
            pManager.AddGenericParameter("Result", "R", "Current iteration result. A non-null value will continue the loop.", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.LoopCompletition, GH_ParamAccess.item);
            pManager.AddGenericParameter("Output", "O", "Data gathered from iterations", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!FindStart())
                return;

            var controlReturnedToStart = _loopStart.TryIncrement(out int iteration);
            var loopFinished = !controlReturnedToStart;
            AdjustComponentName(loopFinished, iteration);

            DA.SetData(ParamInfos.LoopCompletition.Name, loopFinished);

            DA.GetDataTree<IGH_Goo>(1, out GH_Structure<IGH_Goo> dataTree);
            AppendCurrentLoopResult(dataTree, iteration);
            DA.SetDataTree(1, _mergedData);
        }

        private bool FindStart()
        {
            if (Params.Input[0].SourceCount != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Connect single Loop Start with this End");
                return false;
            }

            var potentialStart = Params.Input[0].Sources[0].Attributes.Parent.DocObject;
            if (potentialStart.ComponentGuid != new Guid(VersionSpecificConstants.LoopStart))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Loop Start not found");
                return false;
            }

            _loopStart = potentialStart as LoopStartComponent;
            return true;
        }

        private void AdjustComponentName(bool loopFinished, int iteration)
        {
            if (loopFinished)
            {
                Name = ComponentInfos.LoopEndComponent.Name;
                NickName = ComponentInfos.LoopEndComponent.Name;
            }
            else
            {
                Name = $"Iteration: {iteration}";
                NickName = $"Iteration: {iteration}";
            }
        }

        private void AppendCurrentLoopResult(GH_Structure<IGH_Goo> dataTree, int iteration)
        {
            if (iteration == 0)
                _mergedData.Clear();

            for (int i = 0; i < dataTree.PathCount; i++)
            {
                var initialPath = dataTree.Paths[i];
                var objects = dataTree.Branches[i];

                _mergedData.AppendRange(objects, initialPath.PrependElement(iteration));
            }
        }
    }
}
