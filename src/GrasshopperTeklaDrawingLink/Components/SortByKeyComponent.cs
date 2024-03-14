using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System.Collections.Generic;

namespace GTDrawingLink.Components
{
    public class SortByKeyComponent : TeklaComponentBaseNew<SortByKeyCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SortByKey;
        public SortByKeyComponent() : base(ComponentInfos.SortByKeyComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (TreeData<IGH_Goo> objectsTree, List<string> keys, List<string> sortOrder) = _command.GetInputValues();

            var listMode = objectsTree.Paths.Count == 1;
            if (!CheckInputs(objectsTree, keys, listMode))
                return;

            var indicies = GetOrderedIndicies(keys, sortOrder);

            var output = new GH_Structure<IGH_Goo>();
            if (listMode)
            {
                foreach (var index in indicies)
                    output.Append(objectsTree.Objects[0][index]);
            }
            else
            {
                for (int i = 0; i < indicies.Count; i++)
                {
                    var path = new GH_Path(0, i);
                    output.AppendRange(objectsTree.Objects[indicies[i]], path);
                }
            }

            _command.SetOutputValues(DA, output);
        }

        private bool CheckInputs(TreeData<IGH_Goo> objectsTree, List<string> keys, bool listMode)
        {
            if (listMode)
            {
                if (objectsTree.Objects[0].Count != keys.Count)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of elements must match the number of keys");
                    return false;
                }
            }
            else if (objectsTree.Paths.Count != keys.Count)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of branches must match the number of keys");
                return false;
            }

            return true;
        }

        private List<int> GetOrderedIndicies(List<string> keys, List<string> sortOrder)
        {
            var keysLeft = new List<string>(keys);
            var indicies = new List<int>();
            foreach (var key in sortOrder)
            {
                var index = keys.IndexOf(key);
                if (index != -1)
                {
                    indicies.Add(index);
                    keysLeft.Remove(key);
                }
            }

            foreach (var key in keysLeft)
            {
                indicies.Add(keys.IndexOf(key));
            }

            return indicies;
        }
    }

    public class SortByKeyCommand : CommandBase
    {
        private readonly InputTreeParam<IGH_Goo> _inObjects = new InputTreeParam<IGH_Goo>(ParamInfos.Values);
        private readonly InputListParam<string> _inKeys = new InputListParam<string>(ParamInfos.GroupingKeys);
        private readonly InputListParam<string> _inSortOrder = new InputListParam<string>(ParamInfos.SortOrder);

        private readonly OutputTreeParam<IGH_Goo> _outObjects = new OutputTreeParam<IGH_Goo>(ParamInfos.SortedValues, 0);

        internal (TreeData<IGH_Goo> Inputs, List<string> Keys, List<string> SordOrder) GetInputValues()
        {
            return (_inObjects.AsTreeData(),
                    _inKeys.Value,
                    _inSortOrder.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure sortedObjects)
        {
            _outObjects.Value = sortedObjects;

            return SetOutput(DA);
        }
    }
}
