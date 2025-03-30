using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class SummonHomePortalScript : ReactorTileScriptBase
{
    private readonly ISimpleCache SimpleCache;
    protected IIntervalTimer AnimationTimer { get; set; }

    public string DestinationMapKey { get; set; } = null!;
    public IEffectFactory EffectFactory { get; init; }

    /// <inheritdoc />
    public Point OriginPoint { get; set; }

    protected Creature? Owner { get; set; }
    protected IIntervalTimer? Timer { get; set; }

    protected Animation PortalAnimation { get; } = new()
    {
        AnimationSpeed = 145,
        TargetAnimation = 410
    };

    /// <inheritdoc />
    public SummonHomePortalScript(ReactorTile subject, ISimpleCache simpleCache, IEffectFactory effectFactory)
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
            if (source is Aisling aisling)
            {
                switch (aisling.Nation)
                {
                    case Nation.Exile:
                        OriginPoint = new Point(8, 5);
                        DestinationMapKey = "toc";

                        break;
                    case Nation.Suomi:
                        OriginPoint = new Point(9, 5);
                        DestinationMapKey = "suomi_inn";

                        break;
                    case Nation.Ellas:
                        OriginPoint = new Point(9, 2);

                        break;
                    case Nation.Loures:
                        OriginPoint = new Point(5, 6);
                        DestinationMapKey = "loures_2_floor_empty_room_1";

                        break;
                    case Nation.Mileth:
                        OriginPoint = new Point(4, 8);
                        DestinationMapKey = "mileth_inn";

                        break;
                    case Nation.Tagor:
                        OriginPoint = new Point(4, 8);
                        DestinationMapKey = "tagor_inn";

                        break;
                    case Nation.Rucesion:
                        OriginPoint = new Point(7, 5);
                        DestinationMapKey = "rucesion_inn";

                        break;
                    case Nation.Noes:
                        OriginPoint = new Point(9, 9);

                        break;
                    case Nation.Illuminati:
                        OriginPoint = new Point(9, 10);

                        break;
                    case Nation.Piet:
                        OriginPoint = new Point(5, 8);
                        DestinationMapKey = "piet_inn";

                        break;
                    case Nation.Atlantis:
                        OriginPoint = new Point(9, 12);

                        break;
                    case Nation.Abel:
                        OriginPoint = new Point(4, 7);
                        DestinationMapKey = "abel_inn";

                        break;
                    case Nation.Undine:
                        OriginPoint = new Point(12, 4);
                        DestinationMapKey = "undine_tavern";

                        break;
                    case Nation.Labyrinth:
                        OriginPoint = new Point(6, 7);
                        DestinationMapKey = "arena_entrance";

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var targetMap = SimpleCache.Get<MapInstance>(DestinationMapKey);

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

                aisling.TraverseMap(targetMap, OriginPoint);
                owner.SendActiveMessage($"{aisling.Name} has entered your portal.");
            }

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
                                  .GetEntitiesWithinRange<Aisling>(Subject, 12)
                                  .Where(x => (x.Group != null) && x.Group.Contains(Subject.Owner));

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