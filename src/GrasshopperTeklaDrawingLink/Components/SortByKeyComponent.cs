﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace GTDrawingLink.Components
{
    public class SortByKeyComponent : TeklaComponentBaseNew<SortByKeyCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.SortByKey;
        public SortByKeyComponent() : base(ComponentInfos.SortByKeyComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (TreeData<IGH_Goo> objectsTree, TreeData<string> keys, List<string> sortOrder) = _command.GetInputValues();

            var listMode = objectsTree.Paths.Count == 1;
            if (!CheckInputs(objectsTree, keys, listMode))
                return;

            var mergedKeyes = keys.Objects.Count == 0 ?
                keys.Objects.First() :
                keys.Objects.Select(o => o.First()).ToList();

            var output = new GH_Structure<IGH_Goo>();
            if (listMode)
            {
                var indicies = GetOrderedIndicies(mergedKeyes, sortOrder);
                foreach (var index in indicies)
                    output.Append(objectsTree.Objects[0][index]);
            }
            else
            {
                var indicies = GetOrderedIndicies(mergedKeyes, sortOrder);
                for (int i = 0; i < indicies.Count; i++)
                {
                    var path = new GH_Path(0, i);
                    output.AppendRange(objectsTree.Objects[indicies[i]], path);
                }
            }

            _command.SetOutputValues(DA, output);
        }

        private bool CheckInputs(TreeData<IGH_Goo> objectsTree, TreeData<string> keys, bool listMode)
        {
            if (listMode)
            {
                if (objectsTree.Objects[0].Count != keys.Objects[0].Count)
                {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The number of elements must match the number of keys");
                    return false;
                }
            }
            else if (objectsTree.Paths.Count != keys.Paths.Count)
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
            foreach (var pattern in sortOrder)
            {
                for (int i = 0; i < keysLeft.Count; i++)
                {
                    string key = keysLeft[i];
                    if (MatchCriteria(key, pattern))
                    {
                        indicies.Add(keys.IndexOf(key));
                        keysLeft.Remove(key);
                        i--;
                    }
                }
            }

            foreach (var key in keysLeft)
            {
                indicies.Add(keys.IndexOf(key));
            }

            return indicies;
        }

        private bool MatchCriteria(string key, string pattern)
        {
            return LikeOperator.LikeString(key, pattern, CompareMethod.Binary);
        }
    }

    public class SortByKeyCommand : CommandBase
    {
        private readonly InputTreeParam<IGH_Goo> _inObjects = new InputTreeParam<IGH_Goo>(ParamInfos.Values);
        private readonly InputTreeParam<string> _inKeys = new InputTreeParam<string>(ParamInfos.GroupingKeys);
        private readonly InputListParam<string> _inSortOrder = new InputListParam<string>(ParamInfos.SortOrder);

        private readonly OutputTreeParam<IGH_Goo> _outObjects = new OutputTreeParam<IGH_Goo>(ParamInfos.SortedValues, 0);

        internal (TreeData<IGH_Goo> Inputs, TreeData<string> Keys, List<string> SordOrder) GetInputValues()
        {
            return (_inObjects.AsTreeData(),
                    _inKeys.AsTreeData(),
                    _inSortOrder.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure sortedObjects)
        {
            _outObjects.Value = sortedObjects;

            return SetOutput(DA);
        }
    }
}
