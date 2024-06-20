using Chaos.Collections;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class EingrenEscapePortalScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected Creature? Owner { get; set; }
    protected IIntervalTimer? Timer { get; set; }
    protected IIntervalTimer AnimationTimer { get; set; }
    public IEffectFactory EffectFactory { get; init; }
    
    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 410
    };

    /// <inheritdoc />
    public EingrenEscapePortalScript(ReactorTile subject, ISimpleCache simpleCache, IEffectFactory effectFactory)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Owner = subject.Owner;
        EffectFactory = effectFactory;
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        Timer = new IntervalTimer(TimeSpan.FromSeconds(60), false);
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling)
            return;
        
        var targetMap = SimpleCache.Get<MapInstance>("main_manor_hall");
        var rectangle = new Rectangle(13, 2, 2, 3);
        var point = rectangle.GetRandomPoint();
            
        source.TraverseMap(targetMap, point);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        AnimationTimer.Update(delta);

        if (AnimationTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance
                                           .GetEntitiesWithinRange<Aisling>(Subject, 12);

            foreach (var aisling in aislings)
                aisling.MapInstance.ShowAnimation(PortalAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));
        }
    }
}