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

namespace Chaos.Scripting.ReactorTileScripts.MainStoryQuest;

public class SummonerEscapePortalScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected IIntervalTimer AnimationTimer { get; set; }
    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 410
    };

    /// <inheritdoc />
    public SummonerEscapePortalScript(ReactorTile subject, ISimpleCache simpleCache)
        : base(subject)
    {
        SimpleCache = simpleCache;
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (source is not Aisling aisling)
            return;

        if (Subject.MapInstance.GetEntities<Monster>().Any(x => !x.Script.Is<PetScript>()))
        {
            aisling.SendOrangeBarMessage("You must clear the room before taking the portal.");
            var point = source.DirectionalOffset(source.Direction.Reverse());
            source.WarpTo(point);
            return;
        }

        if (Subject.MapInstance.GetEntities<Aisling>()
            .Any(x => !x.Trackers.Enums.HasValue(MainStoryEnums.StartedSummonerFight)))
            return;

        switch (Subject.MapInstance.Template.MapId)
        {
            case 22100:
            {
                var targetMap = SimpleCache.Get<MapInstance>("terraguardiandomain");
                var rectangle = new Rectangle(10, 13, 3, 3);
                var point = rectangle.GetRandomPoint();

                foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
                {
                    player.TraverseMap(targetMap, point);
                    if (source.Name != player.Name)
                        player.SendOrangeBarMessage($"{source.Name} drags you through the portal.");
                    
                    if (source.Name == player.Name)
                        player.SendOrangeBarMessage("You bring your group through the portal.");
                }
                break;
            }
            
            case 31002:
            {
                var targetMap = SimpleCache.Get<MapInstance>("galeguardiandomain");
                var rectangle = new Rectangle(10, 13, 3, 3);
                var point = rectangle.GetRandomPoint();

                foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
                {
                    player.TraverseMap(targetMap, point);
                    if (source.Name != player.Name)
                        player.SendOrangeBarMessage($"{source.Name} drags you through the portal.");
                    
                    if (source.Name == player.Name)
                        player.SendOrangeBarMessage("You bring your group through the portal.");
                }
                break;
            }
            case 31004:
            {
                var targetMap = SimpleCache.Get<MapInstance>("tideguardiandomain");
                var rectangle = new Rectangle(6, 9, 3, 3);
                var point = rectangle.GetRandomPoint();

                foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
                {
                    player.TraverseMap(targetMap, point);
                    if (source.Name != player.Name)
                        player.SendOrangeBarMessage($"{source.Name} drags you through the portal.");
                    
                    if (source.Name == player.Name)
                        player.SendOrangeBarMessage("You bring your group through the portal.");
                }
                break;
            }
            case 31003:
            {
                var targetMap = SimpleCache.Get<MapInstance>("ignisguardiandomain");
                var rectangle = new Rectangle(13, 8, 3, 3);
                var point = rectangle.GetRandomPoint();

                foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
                {
                    player.TraverseMap(targetMap, point);
                    if (source.Name != player.Name)
                        player.SendOrangeBarMessage($"{source.Name} drags you through the portal.");
                    
                    if (source.Name == player.Name)
                        player.SendOrangeBarMessage("You bring your group through the portal.");
                }
                break;
            }
            case 31001:
            {
                var targetMap = SimpleCache.Get<MapInstance>("cthonic_domain");
                var rectangle = new Rectangle(13, 14, 3, 3);
                var point = rectangle.GetRandomPoint();

                foreach (var player in Subject.MapInstance.GetEntities<Aisling>().Where(x => x.IsAlive).ToList())
                {
                    player.TraverseMap(targetMap, point);
                    if (source.Name != player.Name)
                        player.SendOrangeBarMessage($"{source.Name} drags you through the portal.");
                    
                    if (source.Name == player.Name)
                        player.SendOrangeBarMessage("You bring your group through the portal.");
                }
                break;
            }
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
    }
}