using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
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
            (TreeData<IGH_Goo> objectsTree, TreeData<string> keys, List<string> search) = _command.GetInputValues();

            var inputType = EstablishInputType(objectsTree, keys);
            if (!CheckInputs(objectsTree, keys, inputType))
                return;

            var output = new GH_Structure<IGH_Goo>();
            if (inputType == InputType.Tree)
            {
                var matchingIndicies = GetMatchingPaths(keys, search);
                if (matchingIndicies.Count == 0)
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No matching key found");

                foreach (var path in matchingIndicies)
                {
                    output.AppendRange(objectsTree.Objects[path.index], path.path);
                }
            }
            else if (inputType == InputType.ListWithCorrespondingKeys)
            {
                var matchingIndicies = GetMatchingIndicies(keys.Objects[0], search);
                if (matchingIndicies.Count == 0)
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No matching key found");

                foreach (var path in matchingIndicies)
                    output.Append(objectsTree.Objects[0][path]);
            }
            else if (inputType == InputType.ListWithSingleKey)
            {
                var key = keys.Objects[0][0];
                if (AnyKeyMeetsCriteria(key, search))
                    output.AppendRange(objectsTree.Objects[0]);
            }

            _command.SetOutputValues(DA, output);
        }

        private InputType EstablishInputType(TreeData<IGH_Goo> objectsTree, TreeData<string> keys)
        {
            if (objectsTree.Paths.Count == 1)
            {
                if (keys.Paths.Count == 1 && keys.Objects.First().Count == 1)
                    return InputType.ListWithSingleKey;
                else
                    return InputType.ListWithCorrespondingKeys;
            }
            else
                return InputType.Tree;
        }

        private bool CheckInputs(TreeData<IGH_Goo> objectsTree, TreeData<string> keys, InputType inputType)
        {
            switch (inputType)
            {
                case InputType.Tree:
                    if (objectsTree.Paths.Count != keys.Paths.Count)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of branches must match the number of keys");
                        return false;
                    }
                    break;
                case InputType.ListWithCorrespondingKeys:
                    if (objectsTree.Objects[0].Count != keys.Objects[0].Count)
                    {
                        this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of elements must match the number of keys");
                        return false;
                    }
                    break;
                default:
                    break;
            }

            return true;
        }

        private List<(int index, GH_Path path)> GetMatchingPaths(TreeData<string> keys, List<string> search)
        {
            var paths = new List<(int index, GH_Path)>();

            for (int i = 0; i < keys.Objects.Count; i++)
            {
                var key = keys.Objects[i].First();
                if (AnyKeyMeetsCriteria(key, search))
                    paths.Add((i, keys.Paths[i]));
            }

            return paths;
        }

        private List<int> GetMatchingIndicies(List<string> keys, List<string> search)
        {
            var indicies = new List<int>();
            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                if (AnyKeyMeetsCriteria(key, search))
                    indicies.Add(i);
            }

            return indicies;
        }

        private bool AnyKeyMeetsCriteria(string key, List<string> searchValues)
        {
            return searchValues.Any(s => LikeOperator.LikeString(key, s, CompareMethod.Binary));
        }

        enum InputType
        {
            Tree,
            ListWithCorrespondingKeys,
            ListWithSingleKey
        }
    }

    public class SearchUsingKeyCommand : CommandBase
    {
        private readonly InputTreeParam<IGH_Goo> _inObjects = new InputTreeParam<IGH_Goo>(ParamInfos.Values);
        private readonly InputTreeParam<string> _inKeys = new InputTreeParam<string>(ParamInfos.GroupingKeys);
        private readonly InputListParam<string> _inSearch = new InputListParam<string>(ParamInfos.Search);

        private readonly OutputTreeParam<IGH_Goo> _outObjects = new OutputTreeParam<IGH_Goo>(ParamInfos.FoundedBranches, 0);

        internal (TreeData<IGH_Goo> ObjectsTree, TreeData<string> Keys, List<string> Search) GetInputValues()
        {
            return (_inObjects.AsTreeData(),
                    _inKeys.AsTreeData(),
                    _inSearch.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure found)
        {
            _outObjects.Value = found;

            return SetOutput(DA);
        }
    }
}
