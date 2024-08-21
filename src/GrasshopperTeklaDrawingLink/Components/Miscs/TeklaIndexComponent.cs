using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using TSM = Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Miscs
{
    public class TeklaIndexComponent : TeklaComponentBaseNew<TeklaIndexCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.TeklaIndex;
        public TeklaIndexComponent() : base(ComponentInfos.TeklaIndexComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<IGH_Goo> set, IGH_Goo member) = _command.GetInputValues();

            var id = GetId(member);
            if (id == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided member is not Tekla type");
                return;
            }

            var indicies = new List<int>();
            for (int i = 0; i < set.Count; i++)
            {
                if (id == GetId(set[i]))
                    indicies.Add(i);
            }
            _command.SetOutputValues(DA, indicies, indicies.Count);
        }

        private int GetId(IGH_Goo goo)
        {
            return goo switch
            {
                GH_Goo<TSM.ModelObject> modelObject => modelObject.Value.Identifier.ID,
                GH_Goo<TSD.DatabaseObject> drawingObject => drawingObject.Value.GetId(),
                _ => 0
            };
        }
    }

    public class TeklaIndexCommand : CommandBase
    {
        private readonly InputListParam<IGH_Goo> _inSet = new InputListParam<IGH_Goo>(ParamInfos.Set);
        private readonly InputParam<IGH_Goo> _inMember = new InputParam<IGH_Goo>(ParamInfos.Member);

        private readonly OutputListParam<int> _outIndicies = new OutputListParam<int>(ParamInfos.Index);
        private readonly OutputParam<int> _outCount = new OutputParam<int>(ParamInfos.Count);

        internal (List<IGH_Goo> Set, IGH_Goo Member) GetInputValues()
        {
            return (_inSet.Value,
                    _inMember.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<int> indicies, int count)
        {
            _outIndicies.Value = indicies;
            _outCount.Value = count;

            return SetOutput(DA);
        }
    }
}
