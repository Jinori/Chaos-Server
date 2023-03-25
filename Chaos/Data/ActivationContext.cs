using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Data;

public class ActivationContext
{
    public MapInstance Map { get; }
    public Creature Source { get; }
    public Aisling? SourceAisling { get; }
    public Point SourcePoint => Point.From(Source);
    public Creature Target { get; }
    public Aisling? TargetAisling { get; }
    public Point TargetPoint => Point.From(Target);

    public ActivationContext(Creature source, Creature target)
    {
        Source = source;
        Target = target;
        SourceAisling = Source as Aisling;
        TargetAisling = Target as Aisling;
        Map = Target.MapInstance;
    }
}