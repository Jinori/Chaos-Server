using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using NLog.Targets;

namespace Chaos.Objects;

public sealed record SkillContext(Creature Source)
{
    public Aisling? AislingSource { get; } = Source as Aisling;
    public MapInstance Map { get; } = Source.MapInstance;
    public IPoint SourcePoint { get; } = Point.From(Source);
}