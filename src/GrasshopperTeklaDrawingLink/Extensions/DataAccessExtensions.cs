using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.Collections.Generic;
using System.Linq;

namespace GTDrawingLink.Extensions
{
    internal static class DataAccessExtensions
    {
        public static T GetGooValue<T>(this IGH_DataAccess DA, GH_InstanceDescription instanceDescription)
        {
            GH_Goo<T> objectGoo = null;
            var parameterSet = DA.GetData(instanceDescription.Name, ref objectGoo);
            if (!parameterSet || objectGoo.Value == null)
                return default(T);

            return objectGoo.Value;
        }

        public static List<T> GetGooListValue<T>(this IGH_DataAccess DA, GH_InstanceDescription instanceDescription)
        {
            List<GH_Goo<T>> viewGoo = new List<GH_Goo<T>>();
            var parameterSet = DA.GetDataList(instanceDescription.Name, viewGoo);
            if (!parameterSet)
                return null;

            return viewGoo
                .Where(goo => goo != null && goo.Value != null)
                .Select(goo => goo.Value)
                .ToList();
        }

        public static List<List<T>> GetGooDataTreeValue<T>(this IGH_DataAccess DA, GH_InstanceDescription instanceDescription) where T : IGH_Goo
        {
            var parameterSet = DA.GetDataTree(instanceDescription.Name, out GH_Structure<T> tree);
            if (!parameterSet)
                return null;

            return tree.Branches.Select(b => b).ToList();
        }
    }
}
