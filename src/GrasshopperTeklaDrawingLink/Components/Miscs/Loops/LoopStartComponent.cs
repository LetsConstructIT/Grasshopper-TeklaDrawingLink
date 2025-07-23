using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Drawing;
using System.Linq;

namespace GTDrawingLink.Components.Miscs.Loops
{
    public class LoopStartComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.LoopStart;

        private bool _isActive;
        private int _iteration;
        private int _loopCount;
        private bool _invokedByLoopEnd;

        public bool Completed { get; private set; }

        public LoopStartComponent() : base(ComponentInfos.LoopStartComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddBooleanParameter(pManager, ParamInfos.BooleanToggle, GH_ParamAccess.item);
            pManager.AddGenericParameter("Data", "D", "Data to loop", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new LinkParameter(ParamInfos.LoopStart, GH_ParamAccess.item));
            pManager.AddIntegerParameter("Counter", "C", "Counter", GH_ParamAccess.item);
            pManager.AddGenericParameter("Item", "I", "Current item from input data", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Completed = false;
            var trigger = false;
            if (!DA.GetData(0, ref trigger) || !trigger)
            {
                _iteration = 0;
                _isActive = false;
                return;
            }

            if (!FindEnd())
                return;

            _isActive = true;
            DA.GetDataTree<IGH_Goo>(1, out GH_Structure<IGH_Goo> dataTree);
            _loopCount = GetTotalLoopCount(dataTree);

            if (!_invokedByLoopEnd)
                _iteration = 0;

            _invokedByLoopEnd = false;

            DA.SetData(1, _iteration);
            DA.SetDataTree(2, GetCurrentItem(dataTree));
        }

        internal bool TryIncrement(out int iteration)
        {
            iteration = _iteration;

            if (!_isActive)
                return false;

            if (_iteration + 1 >= _loopCount)
            {
                Completed = true;
                return false;
            }

            _iteration++;

            _invokedByLoopEnd = true;
            ExpireSolution(true);
            return true;
        }

        internal bool IsToggleOn()
        {
            var startToggle = (GH_Boolean)this.Params.Input.First().VolatileData.AllData(true).First();
            return startToggle.Value;
        }

        private int GetTotalLoopCount(GH_Structure<IGH_Goo> dataTree)
        {
            return dataTree.Branches.Count == 1 ?
                dataTree.Branches.First().Count :
                dataTree.Branches.Count;
        }

        private IGH_Structure GetCurrentItem(GH_Structure<IGH_Goo> dataTree)
        {
            GH_Structure<IGH_Goo> output = new GH_Structure<IGH_Goo>();
            switch (dataTree.Branches.Count)
            {
                case 0:
                    break;
                case 1:
                    output.Append(dataTree.Branches.First()[_iteration]);
                    break;
                default:
                    output.AppendRange(dataTree.Branches[_iteration]);
                    break;
            }
            return output;
        }

        private bool FindEnd()
        {
            if (Params.Output[0].Recipients.Count != 1)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Connect single Loop End with this Start");
                return false;
            }

            var potentialEnd = Params.Output[0].Recipients[0].Attributes.Parent.DocObject;
            if (potentialEnd.ComponentGuid != new Guid(VersionSpecificConstants.LoopEnd))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Loop End not found");
                return false;
            }

            return true;
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, LoopRecompute).ToolTipText = ParamInfos.RecomputeObjects.Description;
        }

        protected void LoopRecompute(object sender, EventArgs e)
        {
            _loopCount = 0;
            _iteration = 0;
            _isActive = false;
            base.ExpireSolution(recompute: true);
        }
    }
}
