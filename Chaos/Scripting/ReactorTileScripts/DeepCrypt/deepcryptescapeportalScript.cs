using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.DeepCrypt;

public class deepcryptescapeportalScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected IIntervalTimer AnimationTimer { get; set; }

    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 410
    };

    /// <inheritdoc />
    public deepcryptescapeportalScript(ReactorTile subject, ISimpleCache simpleCache)
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

        var targetMap = SimpleCache.Get<MapInstance>("crypt16");

        var rectangle = new Rectangle(
            5,
            5,
            3,
            3);
        var point = rectangle.GetRandomPoint();

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