using System;
using System.Collections.Generic;

namespace DrawingLink.UI.GH
{
    public class TeklaParams
    {
        public IReadOnlyList<TeklaModelParam> ModelParams { get; }
        public IReadOnlyList<TeklaDrawingParam> DrawingParams { get; }

        public TeklaParams(IReadOnlyList<TeklaModelParam> modelParams, IReadOnlyList<TeklaDrawingParam> drawingParams)
        {
            ModelParams = modelParams ?? throw new ArgumentNullException(nameof(modelParams));
            DrawingParams = drawingParams ?? throw new ArgumentNullException(nameof(drawingParams));
        }
    }
}