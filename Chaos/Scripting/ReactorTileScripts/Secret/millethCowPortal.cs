using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.Secret;

public class milethCowPortal : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected IIntervalTimer AnimationTimer { get; set; }

    protected IIntervalTimer? Timer { get; set; }

    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 728
    };

    /// <inheritdoc />
    public milethCowPortal(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling)
            return;

        var targetMap = SimpleCache.Get<MapInstance>("mileth");

        var point = new Point(25, 18);

        source.TraverseMap(targetMap, point);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        AnimationTimer.Update(delta);

        if (AnimationTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 12);

            foreach (var aisling in aislings)
                aisling.MapInstance.ShowAnimation(PortalAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));
        }
    }
}