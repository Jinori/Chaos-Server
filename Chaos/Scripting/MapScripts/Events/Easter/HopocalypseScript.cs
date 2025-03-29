using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Models.World;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MapScripts.Events;

public sealed class HopocalypseScript : MapScriptBase
{
    private bool AnnounceStart;
    private bool SpawnedBunnies;
    private readonly IIntervalTimer MessageTimer;
    private readonly Point PlayerStartPoint = new(10, 14);
    private readonly IMonsterFactory MonsterFactory;
    private readonly IItemFactory ItemFactory;

    /// <inheritdoc />
    public HopocalypseScript(MapInstance subject, IMonsterFactory monsterFactory, IItemFactory itemFactory)
        : base(subject)
    {
        MessageTimer = new PeriodicMessageTimer(
            TimeSpan.FromSeconds(15),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(1),
            "Get ready to collect eggs in {Time}!",
            SendMessage);
        
        MonsterFactory = monsterFactory;
        ItemFactory = itemFactory;
    }


    private void SendMessage(string message)
    {
        foreach (var player in Subject.GetEntities<Aisling>())
            player.SendServerMessage(ServerMessageType.ActiveMessage, message);
    }
    
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        MessageTimer.Update(delta);

        if (!AnnounceStart)
        {
            foreach (var aisling in Subject.GetEntities<Aisling>())
            {
                if (aisling.Effects.Contains("mount"))
                    aisling.Effects.Dispel("mount");
                
                var point = new Point(aisling.X, aisling.Y);
                if (point != PlayerStartPoint)
                    aisling.WarpTo(PlayerStartPoint);
            }
        }
        
        if (MessageTimer.IntervalElapsed)
        {
            if (!AnnounceStart)
            {
                if (Subject.Name == "Undine Hopocalypse")
                    Subject.Morph("26021");
            
                SendMessage("Get the eggs! Match start!");
                if (!SpawnedBunnies)
                {
                    // Hopscare at (1, 1)
                    var hopscare = MonsterFactory.Create("hopscare", Subject, new Point(1, 1));
                    Subject.AddEntity(hopscare, new Point(1, 1));

                    // Whiskerflip at (19, 1)
                    var whiskerflip = MonsterFactory.Create("whiskerflip", Subject, new Point(19, 1));
                    Subject.AddEntity(whiskerflip, new Point(19, 1));

                    // Petalpounce at (19, 19)
                    var petalpounce = MonsterFactory.Create("petalpounce", Subject, new Point(19, 19));
                    Subject.AddEntity(petalpounce, new Point(19, 19));

                    // Burrowglint at (1, 19)
                    var burrowglint = MonsterFactory.Create("burrowglint", Subject, new Point(1, 19));
                    Subject.AddEntity(burrowglint, new Point(1, 19));

                    SpawnEggs();
                    
                    SpawnedBunnies = true;
                }
                AnnounceStart = true;
            }
        }
    }
    
    private void SpawnEggs()
    {
        var bounds = Subject.Template.Bounds;

        for (var x = bounds.Left; x <= bounds.Right; x++)
        {
            for (var y = bounds.Top; y <= bounds.Bottom; y++)
            {
                var point = new Point(x, y);
                
                if (Subject.Template.IsWall(point) || point is (20, 9) || point is (0, 9))
                    continue;
                
                var isCorner = ((x == 1) && (y == 1))
                               || ((x == 19) && (y == 1))
                               || ((x == 19) && (y == 19))
                               || ((x == 1) && (y == 19));

                if (isCorner)
                {
                    var item = ItemFactory.Create("undinegoldenchickenegg");
                    var groundItem = new GroundItem(item, Subject, point);
                    Subject.AddEntity(groundItem, point);
                }
                else
                {
                    var item = ItemFactory.Create("undinechickenegg");
                    var groundItem = new GroundItem(item, Subject, point);
                    Subject.AddEntity(groundItem, point);
                }
                
            }
        }
    }
}