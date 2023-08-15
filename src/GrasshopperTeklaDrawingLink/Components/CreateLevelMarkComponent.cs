using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateLevelMarkComponent : CreateDatabaseObjectComponentBase
    {
        private const string _defaultAttributes = "standard";

        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.LevelMark;

        public CreateLevelMarkComponent() : base(ComponentInfos.CreateLevelMarkComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
            pManager.AddPointParameter("Insertion point", "IP", "Insertion point of the Level Mark", GH_ParamAccess.list);
            pManager.AddPointParameter("Base point", "BP", "Base point of the Level Mark", GH_ParamAccess.list);
            pManager.AddTextParameter("Mark attributes", "MA", "Level mark attributes file name", GH_ParamAccess.list, _defaultAttributes);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Mark, GH_ParamAccess.list);
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return null;

            var insertionPoints = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("Insertion point", insertionPoints))
                return null;

            var basePoints = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("Base point", basePoints))
                return null;

            var levelMarkAttributesFileNames = new List<string>();
            DA.GetDataList("Mark attributes", levelMarkAttributesFileNames);

            var levelMarksNumber = new int[] { insertionPoints.Count, basePoints.Count, levelMarkAttributesFileNames.Count }.Max();
            var createdLevelMarks = new LevelMark[levelMarksNumber];
            for (int i = 0; i < levelMarksNumber; i++)
            {
                var levelMark = InsertLevelMark(
                    view,
                    insertionPoints.ElementAtOrLast(i),
                    basePoints.ElementAtOrLast(i),
                    levelMarkAttributesFileNames.Count > 0 ? levelMarkAttributesFileNames.ElementAtOrLast(i) : _defaultAttributes);

                createdLevelMarks[i] = levelMark;
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.Mark.Name, createdLevelMarks.Select(l => new TeklaDatabaseObjectGoo(l)));

            return createdLevelMarks;
        }

        protected LevelMark InsertLevelMark(View view,
                                            Rhino.Geometry.Point3d insertionPoint,
                                            Rhino.Geometry.Point3d basePoint,
                                            string levelMarkAttributesFileName)
        {
            var levelMarkAttributes = new LevelMark.LevelMarkAttributes();
            if (!string.IsNullOrEmpty(levelMarkAttributesFileName))
                levelMarkAttributes.LoadAttributes(levelMarkAttributesFileName);

            var levelMark = new LevelMark(
                view,
                insertionPoint.ToTekla(),
                basePoint.ToTekla(),
                levelMarkAttributes);

            levelMark.Insert();

            return levelMark;
        }
    }
}
