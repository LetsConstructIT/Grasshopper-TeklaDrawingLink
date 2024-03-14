using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Components
{
    public class SearchUsingKeyComponent : TeklaComponentBaseNew<SearchUsingKeyCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SearchUsingKey;
        public SearchUsingKeyComponent() : base(ComponentInfos.SearchUsingKeyComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (TreeData<IGH_Goo> objectsTree, List<string> keys, List<string> search) = _command.GetInputValues();

            var listMode = objectsTree.Paths.Count == 1;
            if (!CheckInputs(objectsTree, keys, listMode))
                return;

            var matchingIndicies = GetMatchingIndicies(keys, search);
            if (matchingIndicies.Count == 0)
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No matching key found");

            var output = new GH_Structure<IGH_Goo>();
            if (listMode)
            {
                foreach (var index in matchingIndicies)
                    output.Append(objectsTree.Objects[0][index]);
            }
            else
            {
                foreach (var index in matchingIndicies)
                {
                    var path = new GH_Path(0, index);
                    output.AppendRange(objectsTree.Objects[index], path);
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

        private List<int> GetMatchingIndicies(List<string> keys, List<string> search)
        {
            var indicies = new List<int>();
            for (int i = 0; i < keys.Count; i++)
            {
                if (search.Any(s => s.Equals(keys[i])))
                    indicies.Add(i);
            }

            return indicies;
        }
    }

    public class SearchUsingKeyCommand : CommandBase
    {
        private readonly InputTreeParam<IGH_Goo> _inObjects = new InputTreeParam<IGH_Goo>(ParamInfos.Values);
        private readonly InputListParam<string> _inKeys = new InputListParam<string>(ParamInfos.GroupingKeys);
        private readonly InputListParam<string> _inSearch = new InputListParam<string>(ParamInfos.Search);

        private readonly OutputTreeParam<IGH_Goo> _outObjects = new OutputTreeParam<IGH_Goo>(ParamInfos.FoundedBranches, 0);

        internal (TreeData<IGH_Goo> ObjectsTree, List<string> Keys, List<string> Search) GetInputValues()
        {
            return (_inObjects.AsTreeData(),
                    _inKeys.Value,
                    _inSearch.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure found)
        {
            _outObjects.Value = found;

            return SetOutput(DA);
        }
    }
}
