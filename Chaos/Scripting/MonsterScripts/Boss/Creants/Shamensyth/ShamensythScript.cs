using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Creants.Shamensyth;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Shamensyth;

public class ShamensythScript : MonsterScriptBase
{
    //room slowly catches on fire, more and more tiles get fire on them
        //if player steps on fire tile, they get a burn effect that deals damage over time
        //fire tiles can be put out by using an aoe water spell
    //teleport phase:
        //boss teleports to certain spots every 15s, uses ranged attacks
        //auto attacks with ard srad
        //will cast reign of fire when nobody is near him (casting uninterrupted)
    //room aoe phase:
        //boss teleports to center of room, begins chanting
        //at end of chant (3 or 4 lines maybe?) boss does a room aoe that doesnt go through walls
        //players need to hide behind pillars
        //12,12
    //spawn adds phase:
        //boss spawns 4 fire elementals (they are casters, they run from melee)
        //the fire elementals cast ard srad meall on players
    //when it hits 15%, boss will do a reverse room aoe
        //same as room aoe, except shape is inverted
        //players need to hide on inside part of wall
        //chant will be reversed (letters and line order)
        //boss will say say a line something like "time to switch things up" or something
    
    private enum Phase
    {
        Teleport,
        RoomAoe
    }
    private Phase CurrentPhase = Phase.Teleport;
    
    private readonly List<Point> TeleportPoints =
    [
        new(8, 8),
        new(15, 8),
        new(15, 15),
        new(8, 15)
    ];

    private readonly List<Point> AddSpawnPoints =
    [
        new(10, 13),
        new(10, 10),
        new(13, 10),
        new(13, 13)
    ];

    private readonly List<string> ChantLines =
    [
        "Slumbering embers of apocalypse",
        "Radiant scorn of the sun",
        "Let existence tremble",
        "Ashen queen of conflagration",
        "Engulf. Consume.",
        "Cremate"
    ];

    private readonly Point RoomAoePoint = new(12, 12);
    private bool GaveUninterruptedCastWarning;
    private int ChantLineIndex;
    private int SpawnAddsCount;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IReactorTileFactory ReactorTileFactory;
    private bool InvertedRoomAoe;
    
    #region Timers
    private readonly IIntervalTimer TeleportTimer = new RandomizedIntervalTimer(
        TimeSpan.FromSeconds(15),
        15,
        RandomizationType.Positive,
        false);
    private readonly IIntervalTimer ReignOfFireTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly IIntervalTimer TeleportPhaseTimer = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    private readonly IIntervalTimer ChantTimer = new IntervalTimer(TimeSpan.FromMilliseconds(2000), false);
    private readonly IIntervalTimer AfterChantTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
    private readonly IIntervalTimer SpawnAddsTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    private readonly IIntervalTimer BurningGroundTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
    private readonly IIntervalTimer TurnTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly SequentialEventTimer RoomeAoePhaseTimer;
    #endregion
    
    #region Spells
    private readonly Spell ArdSradMeall;
    private readonly Spell ArdSrad;
    private readonly Spell ReignOfFire;
    private readonly Spell Cataclysm;
    private readonly Spell ReverseCataclysm;
    #endregion

    /// <inheritdoc />
    public ShamensythScript(
        Monster subject,
        ISpellFactory spellFactory,
        IMonsterFactory monsterFactory,
        IReactorTileFactory reactorTileFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ReactorTileFactory = reactorTileFactory;
        ArdSrad = spellFactory.Create("ardsrad");
        ArdSradMeall = spellFactory.Create("ardsradmeall");
        ReignOfFire = spellFactory.Create("sham_reignoffire");
        Cataclysm = spellFactory.Create("sham_cataclysm");
        ReverseCataclysm = spellFactory.Create("sham_reverseCataclysm");

        RoomeAoePhaseTimer = new SequentialEventTimer(
            Enumerable.Repeat(ChantTimer, ChantLines.Count)
                      .Append(AfterChantTimer)
                      .ToList());
    }

    private void SpawnAdds(int count)
    {
        var adds = AddSpawnPoints.Shuffle()
                                 .Take(count)
                                 .Select(pt => MonsterFactory.Create("shamensythFireElemental", Subject.MapInstance, pt))
                                 .ToList();

        Subject.MapInstance.AddEntities(adds);
    }

    private void HandleBurningGround()
    {
        if (BurningGroundTimer.IntervalElapsed)
        {
            var existingBurningGround = Subject.MapInstance
                                               .GetEntities<ReactorTile>()
                                               .Where(rt => rt.Script.Is<BurningGroundScript>())
                                               .ToHashSet(PointEqualityComparer.Instance);

            //add 20 burning ground tiles on spots not already occupied by burning ground
            for (var i = 0; i < 20; i++)
                if (Subject.MapInstance.Template.Bounds.TryGetRandomPoint(IsValidBurningGroundPoint, out var pt))
                {
                    var newBurningGround = ReactorTileFactory.Create(
                        "sham_burningGround",
                        Subject.MapInstance,
                        pt,
                        owner: Subject);

                    existingBurningGround.Add(newBurningGround);
                    Subject.MapInstance.SimpleAdd(newBurningGround);
                }

            bool IsValidBurningGroundPoint(Point pt)
            {
                if (existingBurningGround.Contains(pt))
                    return false;

                if (Subject.MapInstance.IsWall(pt))
                    return false;

                return true;
            }
        }
        

    }

    private void HandleAdds()
    {
        if(SpawnAddsTimer.IntervalElapsed)
            switch (SpawnAddsCount)
            {
                case 0:
                {
                    if ((int)Subject.StatSheet.HealthPercent <= 85)
                    {
                        SpawnAdds(1);
                        SpawnAddsCount++;
                    }

                    break;
                }
                case 1:
                {
                    if ((int)Subject.StatSheet.HealthPercent <= 70)
                    {
                        SpawnAdds(2);
                        SpawnAddsCount++;
                    }

                    break;
                }
                case 2:
                {
                    if ((int)Subject.StatSheet.HealthPercent <= 55)
                    {
                        SpawnAdds(3);
                        SpawnAddsCount++;
                    }

                    break;
                }
                case 3:
                {
                    if ((int)Subject.StatSheet.HealthPercent <= 40)
                    {
                        SpawnAdds(4);
                        SpawnAddsCount++;
                    }

                    break;
                }
                case 4:
                {
                    if ((int)Subject.StatSheet.HealthPercent <= 25)
                    {
                        SpawnAdds(4);
                        SpawnAddsCount++;
                        InvertedRoomAoe = true;

                        Subject.Say("Time to switch things up...");
                    }

                    break;
                }
                case 5:
                {
                    if ((int)Subject.StatSheet.HealthPercent <= 15)
                    {
                        SpawnAdds(4);
                        SpawnAddsCount++;
                    }

                    break;
                }
            }
    }

    private void HandleRoomAoe()
    {
        if (!InvertedRoomAoe)
        {
            if (RoomeAoePhaseTimer.IntervalElapsed)
                if (RoomeAoePhaseTimer.CurrentTimer == ChantTimer)
                {
                    if (ChantLineIndex < ChantLines.Count)
                    {
                        Subject.Shout(ChantLines[ChantLineIndex]);
                        ChantLineIndex++;

                        if (ChantLineIndex == ChantLines.Count)
                            Subject.TryUseSpell(Cataclysm);
                    }
                } else if (RoomeAoePhaseTimer.CurrentTimer == AfterChantTimer)
                {
                    CurrentPhase = Phase.Teleport;
                    ChantLineIndex = 0;
                    RoomeAoePhaseTimer.Reset();
                }
        } else
        {
            if (RoomeAoePhaseTimer.IntervalElapsed)
                if (RoomeAoePhaseTimer.CurrentTimer == ChantTimer)
                {
                    if (ChantLineIndex < ChantLines.Count)
                    {
                        Subject.Shout(new string(ChantLines[ChantLines.Count - ChantLineIndex - 1]));
                        ChantLineIndex++;

                        if (ChantLineIndex == ChantLines.Count)
                            Subject.TryUseSpell(ReverseCataclysm);
                    }
                } else if (RoomeAoePhaseTimer.CurrentTimer == AfterChantTimer)
                {
                    CurrentPhase = Phase.Teleport;
                    ChantLineIndex = 0;
                    RoomeAoePhaseTimer.Reset();
                }
        }
    }

    private void HandleReignOfFire()
    {
        var nearbyAisling = Subject.MapInstance.GetEntitiesWithinRange<Aisling>(Subject, 1);

        if (!nearbyAisling.Any())
        {
            if (!GaveUninterruptedCastWarning)
            {
                foreach (var aisling in Subject.MapInstance.GetEntities<Aisling>())
                    aisling.SendActiveMessage("The shamensyth is able to cast without interruption!");

                GaveUninterruptedCastWarning = true;
            }

            Subject.TryUseSpell(ReignOfFire);
        }
    }

    private void HandleFacing()
    {
        if (CurrentPhase == Phase.Teleport)
        {
            if (TurnTimer.IntervalElapsed && Subject.Target is not null)
            {
                var direction = Subject.Target.DirectionalRelationTo(Subject);
                Subject.Turn(direction);
            }
        } else if (Subject.Direction != Direction.Down)
            Subject.Turn(Direction.Down);
    }

    private void HandleTeleport()
    {
        var possiblePoints = TeleportPoints.Where(pt => !pt.Equals(Subject))
                                           .ToList();

        var targetPoint = possiblePoints.PickRandom();

        Subject.WarpTo(targetPoint);
        ReignOfFireTimer.Reset();
    }
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        SpawnAddsTimer.Update(delta);
        BurningGroundTimer.Update(delta);
        TurnTimer.Update(delta);
        
        ArdSradMeall.Update(delta);
        ArdSrad.Update(delta);
        
        HandleFacing();
        HandleAdds();
        HandleBurningGround();
        
        switch (CurrentPhase)
        {
            case Phase.Teleport:
            {
                TeleportPhaseTimer.Update(delta);
                ReignOfFireTimer.Update(delta);
                TeleportTimer.Update(delta);
                Subject.SpellTimer.Update(delta);

                if (TeleportTimer.IntervalElapsed)
                    HandleTeleport();

                //basically his autoattack
                if (Subject.SpellTimer.IntervalElapsed)
                    if (Subject.Target is not null)
                        if (!Subject.TryUseSpell(ArdSrad, Subject.Target.Id))
                            Subject.TryUseSpell(ArdSradMeall, Subject.Target.Id);

                if (ReignOfFireTimer.IntervalElapsed)
                    HandleReignOfFire();
                
                //next phase
                if (TeleportPhaseTimer.IntervalElapsed)
                {
                    CurrentPhase = Phase.RoomAoe;
                    TeleportPhaseTimer.Reset();
                }

                break;
            }
            case Phase.RoomAoe:
            {
                if (!RoomAoePoint.Equals(Subject))
                    Subject.WarpTo(RoomAoePoint);
                
                RoomeAoePhaseTimer.Update(delta);
                HandleRoomAoe();
                
                break;
            }
        }
    }
}