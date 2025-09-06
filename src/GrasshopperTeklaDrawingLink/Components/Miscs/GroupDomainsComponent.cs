using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Components.Miscs
{
    public class GroupDomainsComponent : TeklaComponentBaseNew<GroupDomainsCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override System.Drawing.Bitmap Icon => Properties.Resources.GroupDomains;

        public GroupDomainsComponent() : base(ComponentInfos.GroupDomainsComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var targetPath = DA.ParameterTargetPath(0);
            var inputIntervals = _command.GetInputValues();

            var intervals = inputIntervals.Select(i => i.Value).ToList();
            var grouped = IntervalGrouper.GroupIntervals(intervals);

            var outputIntervals = new GH_Structure<GH_Interval>();
            var outputSummaryIntervals = new GH_Structure<GH_Interval>();
            var outputPaths = new GH_Structure<GH_StructurePath>();
            var outputIndicies = new GH_Structure<GH_Integer>();
            for (int i = 0; i < grouped.Count; i++)
            {
                var resultedIntervals = grouped[i];
                var summary = new Interval(resultedIntervals.Select(i => i.Min).Min(), resultedIntervals.Select(i => i.Max).Max());
                var path = targetPath.AppendElement(i);
                outputIntervals.AppendRange(resultedIntervals.Select(interval => new GH_Interval(interval)), path);
                outputSummaryIntervals.Append(new GH_Interval(summary), path);

                for (int j = 0; j < resultedIntervals.Count; j++)
                {
                    var idx = intervals.IndexOf(resultedIntervals[j]);

                    outputPaths.Append(new GH_StructurePath(targetPath), path);
                    outputIndicies.Append(new GH_Integer(idx), path);
                }
            }

            _command.SetOutputValues(DA, outputIntervals, outputSummaryIntervals, outputPaths, outputIndicies);
        }
    }

    public class IntervalGrouper
    {
        public static List<List<Interval>> GroupIntervals(List<Interval> intervals)
        {
            var result = new List<List<Interval>>();
            var visited = new HashSet<Interval>();

            foreach (var interval in intervals)
            {
                if (visited.Contains(interval))
                    continue;

                // Start new group
                var group = new List<Interval>();
                DFS(interval, intervals, visited, group);
                result.Add(group);
            }

            return result;
        }

        private static void DFS(Interval current, List<Interval> all, HashSet<Interval> visited, List<Interval> group)
        {
            if (!visited.Add(current))
                return;

            group.Add(current);

            foreach (var other in all)
            {
                if (!visited.Contains(other) && DoesIntersect(current, other))
                {
                    DFS(other, all, visited, group);
                }
            }
        }

        private static bool DoesIntersect(Interval intervalA, Interval intervalB)
        {
            var result = Interval.FromIntersection(intervalA, intervalB);
            return result != Interval.Unset;
        }
    }

    public class GroupDomainsCommand : CommandBase
    {
        private readonly InputListParam<GH_Interval> _inDomains = new InputListParam<GH_Interval>(ParamInfos.Domain);

        private readonly OutputTreeParam<GH_Interval> _outDomains = new OutputTreeParam<GH_Interval>(ParamInfos.Domain, 0);
        private readonly OutputTreeParam<GH_Interval> _outSummaryDomains = new OutputTreeParam<GH_Interval>(ParamInfos.SummaryDomain, 1);
        private readonly OutputTreeParam<GH_StructurePath> _outPaths = new OutputTreeParam<GH_StructurePath>(ParamInfos.GhPath, 2);
        private readonly OutputTreeParam<GH_Integer> _outIndices = new OutputTreeParam<GH_Integer>(ParamInfos.Index, 3);


        internal List<GH_Interval> GetInputValues()
        {
            return _inDomains.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, IGH_Structure domains, IGH_Structure domainsSummary, IGH_Structure paths, IGH_Structure indicies)
        {
            _outDomains.Value = domains;
            _outSummaryDomains.Value = domainsSummary;
            _outPaths.Value = paths;
            _outIndices.Value = indicies;

            return SetOutput(DA);
        }
    }
}
