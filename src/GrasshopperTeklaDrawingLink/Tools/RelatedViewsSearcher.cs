using GTDrawingLink.Extensions;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Tools
{
    internal class RelatedViewsSearcher
    {
        private readonly View _sourceView;
        private readonly DrawingHandler _dh;
        private readonly Drawing _drawing;

        public RelatedViewsSearcher(View sourceView)
        {
            _sourceView = sourceView;
            _dh = new DrawingHandler();
            _drawing = _dh.GetActiveDrawing();
        }

        public (List<View> details, List<View> sections) GetRelatedViews()
        {
            int sourceViewId = _sourceView.GetId();

            var relatedDetails = new List<View>();
            foreach (var detailView in GetAllViewsWithType(View.ViewTypes.DetailView))
            {
                var detailSource = GetSourceViewFromSectionByType(detailView, typeof(DetailMark));
                if (detailSource != null && detailSource.GetId() == sourceViewId)
                    relatedDetails.Add(detailView);
            }

            var relatedSections = new List<View>();
            foreach (var sectionView in GetAllViewsWithType(View.ViewTypes.SectionView))
            {
                var sectionSource = GetSourceViewFromSectionByType(sectionView, typeof(SectionMark));
                if (sectionSource != null && sectionSource.GetId() == sourceViewId)
                    relatedSections.Add(sectionView);
            }

            return (relatedDetails, relatedSections);
        }

        private View GetSourceViewFromSectionByType(View childView, Type typeOfConnectorObject)
        {
            DrawingObjectEnumerator doe = childView.GetRelatedObjects(new Type[] { typeOfConnectorObject });
            while (doe.MoveNext())
                return doe.Current.GetView() as View;

            return null;
        }

        private IEnumerable<View> GetAllViewsWithType(View.ViewTypes viewType)
        {
            foreach (var item in _drawing.GetSheet().GetAllViews())
            {
                if (item is View)
                {
                    View view = (View)item;
                    if (view.ViewType == viewType)
                        yield return view;
                }
            }
        }
    }
}
