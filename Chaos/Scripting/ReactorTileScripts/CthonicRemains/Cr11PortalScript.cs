using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Pet;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts.CthonicRemains;

public class Cr11PortalScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    
    protected Creature? Owner { get; set; }
    protected IIntervalTimer AnimationTimer { get; set; }
    protected IIntervalTimer? Timer { get; set; }
    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 410
    };

    /// <inheritdoc />
    public Cr11PortalScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        Owner = subject.Owner;
        Timer = new IntervalTimer(TimeSpan.FromSeconds(120), false);
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (Subject.Owner is Aisling owner)
        {
            var point = new Point(20, 37);
            var targetMap = SimpleCache.Get<MapInstance>("cr11");
            var aisling = source as Aisling;

            if (aisling?.Name == owner.Name)
            {
                aisling.TraverseMap(targetMap, point);
                aisling.SendOrangeBarMessage("You walked through the portal to Cthonic Remains 11.");
                return;
            }

            if ((aisling?.Group != null) && !aisling.Group.Contains(owner))
            {
                aisling.SendOrangeBarMessage("This portal is for another group.");
                return;
            }

            if (aisling?.Group == null)
            {
                aisling?.SendOrangeBarMessage("You must be grouped with the player who opened the portal.");
                return;
            }

            if (!aisling.Trackers.Enums.HasValue(MainStoryEnums.CompletedPreMasterMainStory))
            {
                aisling?.SendOrangeBarMessage("You must speak to Goddess Miraelis after defeating the Summoner.");
                return;
            }
            
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
            
            aisling.TraverseMap(targetMap, point);
            aisling.SendOrangeBarMessage($"You walk through {owner.Name}'s portal to Cthonic Remains 11");
            
            owner.SendActiveMessage($"{aisling.Name} has entered your portal to Cthonic Remains 11.");
        }
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