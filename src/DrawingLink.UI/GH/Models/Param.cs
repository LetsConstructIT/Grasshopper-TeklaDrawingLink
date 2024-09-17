using System;

namespace DrawingLink.UI.GH.Models
{
    public abstract class Param
    {
        public string Name { get; }
        public float Top { get; }

        protected Param(string name, float top)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Top = top;
        }
    }
}
