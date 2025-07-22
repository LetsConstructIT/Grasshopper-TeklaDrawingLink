using Grasshopper.Kernel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Solid;

namespace DrawingLink.UI.GH;
internal class Loops : IEnumerable<Loop>
{
    private readonly List<Loop> _loops = new();

    public IEnumerator<Loop> GetEnumerator()
        => _loops.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _loops.GetEnumerator();

    public void Add(Loop loop)
    {
        _loops.Add(loop);
    }

    public IReadOnlyCollection<IGH_ActiveObject> GetAllObjects()
    {
        var result = new List<IGH_ActiveObject>();
        foreach (var loop in _loops)
        {
            result.Add(loop.LoopStart);
            result.Add(loop.LoopEnd);
        }
        return result;
    }


    private static readonly Guid _loopStart = new("3368FCF5-A321-4B54-944E-36A20DD01ED0");
    private static readonly Guid _loopEnd = new("13DC5668-65C5-43E3-A1EF-F9099205B878");

    public object Current => throw new NotImplementedException();

    internal static IReadOnlyCollection<IGH_Component> FindLoopStarts(List<IGH_ActiveObject> activeObjects)
        => activeObjects.Where(o => o.ComponentGuid == _loopStart).OfType<IGH_Component>().ToList();
    internal static IReadOnlyCollection<IGH_Component> FindLoopEnds(List<IGH_ActiveObject> activeObjects)
        => activeObjects.Where(o => o.ComponentGuid == _loopEnd).OfType<IGH_Component>().ToList();

}

internal class Loop
{
    public IGH_Component LoopStart { get; }
    public IGH_Component LoopEnd { get; }
    public bool HasFinished { get; private set; }
    public int Iteration { get; private set; }

    public Loop(IGH_Component loopStart, IGH_Component loopEnd)
    {
        LoopStart = loopStart ?? throw new ArgumentNullException(nameof(loopStart));
        LoopEnd = loopEnd ?? throw new ArgumentNullException(nameof(loopEnd));

        Iteration = 1;
    }

    public void IncreaseIteration()
    {
        Iteration++;
    }

    public void MarkAsCompleted()
    {
        HasFinished = true;
    }

    internal void ResetComponents()
    {
        LoopStart.Phase = GH_SolutionPhase.Blank;
        LoopEnd.Phase = GH_SolutionPhase.Blank;
    }
}