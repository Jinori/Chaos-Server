using Chaos.Collections;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class SummonPortalScript : ReactorTileScriptBase
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
    public SummonPortalScript(ReactorTile subject, ISimpleCache simpleCache, IEffectFactory effectFactory)
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
        if (Subject.Owner is Aisling owner)
        {
            var targetMap = SimpleCache.Get<MapInstance>(owner.MapInstance.InstanceId);
            var aisling = source as Aisling;
        
            if ((aisling?.Group != null) && !aisling.Group.Contains(owner))
                return;

            if (aisling?.Group == null)
                return;

            if (source.StatSheet.Level < (targetMap.MinimumLevel ?? 0))
            {
                aisling.SendOrangeBarMessage($"You must be at least level {targetMap.MinimumLevel} to enter this area.");
                return;
            }

            if (source.StatSheet.Level > (targetMap.MaximumLevel ?? int.MaxValue))
            {
                aisling.SendOrangeBarMessage($"You must be at most level {targetMap.MaximumLevel} to enter this area.");
                return;
            }
            
            aisling.TraverseMap(targetMap, owner);
            owner.SendActiveMessage($"{aisling.Name} has entered your portal.");
            Map.RemoveEntity(Subject);            
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        AnimationTimer.Update(delta);

        if (AnimationTimer.IntervalElapsed)
        {
            var aislings = Subject.MapInstance
                                           .GetEntitiesWithinRange<Aisling>(Subject, 12).Where(x => (x.Group != null) && x.Group.Contains(Subject.Owner));

            foreach (var aisling in aislings)
                aisling.MapInstance.ShowAnimation(PortalAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));
        }

        if (Subject.Owner is Aisling { Client.Connected: false })
            Map.RemoveEntity(Subject);

        if (Timer != null)
        {
            Timer.Update(delta);

            if (Timer.IntervalElapsed)
                Map.RemoveEntity(Subject);
        }
    }
}