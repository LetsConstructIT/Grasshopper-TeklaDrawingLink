using System;

namespace DrawingLink.UI.GH.Models
{
    public class SliderParam : PersistableParam
    {
        public double Minimum { get; }
        public double Maximum { get; }
        public double Value { get; }
        public int DecimalPlaces { get; }

        public SliderParam(string fieldName, string name, double minValue, double maxValue, double value, int decimalPlaces, float top) : base(fieldName, name, top)
        {
            Minimum = minValue;
            Maximum = maxValue;
            Value = value;
            DecimalPlaces = decimalPlaces;
        }

        public double GetSmallChange()
            => Math.Pow(10.0, -DecimalPlaces);

        public double GetLargeChange()
            => 0.01 * Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(Maximum - Minimum) / 0.9)));
    }
}
