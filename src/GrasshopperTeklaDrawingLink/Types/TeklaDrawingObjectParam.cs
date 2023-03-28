using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Types
{
    public class TeklaDrawingObjectParam : TeklaParamBase<DrawingObject>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.DrawingObject;

        private IEnumerable<Type> _types;
        protected bool _isFloatingParam;

        public override string TypeName
        {
            get
            {
                if (_types != null)
                {
                    return _types.First().ToShortString();
                }
                return typeof(DrawingObject).ToShortString();
            }
        }

        public TeklaDrawingObjectParam(GH_InstanceDescription tag, params Type[] types)
            : base(tag)
        {
            if (types.Any())
            {
                _types = types;
            }
            else
            {
                _types = null;
            }
        }

        public TeklaDrawingObjectParam(GH_InstanceDescription tag, GH_ParamAccess access, params Type[] types)
            : this(tag, types)
        {
            Access = access;
        }

        public override void CreateAttributes()
        {
            m_attributes = new TeklaDrawingObjectParamAttributes(this);
        }

        protected override GH_Goo<DrawingObject> InstantiateT()
        {
            return new TeklaDrawingObjectGoo();
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_Goo<DrawingObject> value)
        {
            var drawingObject = DrawingInteractor.PickObject();

            if (drawingObject == null)
                return GH_GetterResult.cancel;

            value = new TeklaDrawingObjectGoo(drawingObject);
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Plural(ref List<GH_Goo<DrawingObject>> values)
        {
            var drawingObjects = DrawingInteractor.PickObjects();

            if (drawingObjects == null)
                return GH_GetterResult.cancel;

            values = new List<GH_Goo<DrawingObject>>();
            foreach (var drawingObject in drawingObjects)
                values.Add(new TeklaDrawingObjectGoo(drawingObject));

            return GH_GetterResult.success;
        }
        public void HighlightObjects()
        {
            DrawingInteractor.Highlight(GetReferencedModelObjects());
        }

        public void UnHighlightObjects()
        {
            DrawingInteractor.Highlight(null);
        }
        public virtual IEnumerable<DrawingObject> GetReferencedModelObjects()
        {
            return from d in base.VolatileData.AllData(skipNulls: true)
                   select ((GH_Goo<DrawingObject>)d).Value;
        }
    }
}
