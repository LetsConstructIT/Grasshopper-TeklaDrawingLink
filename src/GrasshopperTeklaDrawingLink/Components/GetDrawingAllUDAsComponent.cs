using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingAllUDAsComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetAllUDAs;

        public GetDrawingAllUDAsComponent() : base(ComponentInfos.GetDrawingAllUDAsComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.TeklaDatabaseObject, GH_ParamAccess.list));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(ParamInfos.UDAsOutput.Name, ParamInfos.UDAsOutput.NickName, ParamInfos.UDAsOutput.Description, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var databaseObjects = DA.GetGooListValue<DatabaseObject>(ParamInfos.TeklaDatabaseObject);
            if (databaseObjects == null || !databaseObjects.Any())
                return;

            List<Attributes> attributes = new List<Attributes>();
            foreach (DatabaseObject modelObject in databaseObjects)
            {
                Dictionary<string, string> stringDict;
                Dictionary<string, int> intDict;
                Dictionary<string, double> dblDict;

                if (modelObject is Plugin plugin)
                {
                    stringDict = typeof(Plugin).GetField("LocalStringValues", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(plugin) as Dictionary<string, string>;

                    if (stringDict.Any())
                    {
                        var attribute = Tools.Attributes.Parse(stringDict);
                        attributes.Add(attribute);
                    }

                    intDict = typeof(Plugin).GetField("LocalIntValues", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(plugin) as Dictionary<string, int>;

                    if (intDict.Any())
                    {
                        var attribute = Tools.Attributes.Parse(intDict);
                        attributes.Add(attribute);
                    }

                    dblDict = typeof(Plugin).GetField("LocalDoubleValues", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(plugin) as Dictionary<string, double>;

                    if (dblDict.Any())
                    {
                        var attribute = Tools.Attributes.Parse(dblDict);
                        if (attribute.Count > 0)
                            attributes.Add(attribute);
                    }
                }
                else
                {
                    if (modelObject.GetStringUserProperties(out stringDict) && stringDict.Any())
                        attributes.Add(Tools.Attributes.Parse(stringDict));

                    if (modelObject.GetIntegerUserProperties(out intDict) && intDict.Any())
                        attributes.Add(Tools.Attributes.Parse(intDict));

                    if (modelObject.GetDoubleUserProperties(out dblDict) && dblDict.Any())
                        attributes.Add(Tools.Attributes.Parse(dblDict));
                }
            }

            DA.SetDataList(ParamInfos.UDAsOutput.Name, attributes);
        }
    }
}
