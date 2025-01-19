using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Data;
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
        new(11, 11),
        new(18, 11),
        new(18, 18),
        new(11, 18)
    ];

    private readonly List<Point> AddSpawnPoints =
    [
        new(13, 16),
        new(13, 13),
        new(16, 13),
        new(16, 16)
    ];

    private readonly List<Point> SafePoints;
    private readonly List<Point> InvertedSafePoints;

    private readonly List<string> ChantLines =
    [
        "Slumbering embers of apocalypse",
        "Radiant scorn of the sun",
        "Let existence tremble",
        "Ashen queen of conflagration",
        "Engulf. Consume.",
        "Cremate"
    ];

    #region Animations
    private readonly Animation SafePointAnimation = new()
    {
        TargetAnimation = 214,
        AnimationSpeed = 200
    };
    
    private readonly Animation RoomAoeAnimation1 = new()
    {
        TargetAnimation = 90,
        AnimationSpeed = 750
    };

    private readonly Animation RoomAoeAnimation2 = new()
    {
        TargetAnimation = 52,
        AnimationSpeed = 850
    };

    private readonly Animation RoomAoeAnimation3 = new()
    {
        TargetAnimation = 223,
        AnimationSpeed = 50
    };
    #endregion

    private readonly Point RoomAoePoint = new(15, 15);
    private bool GaveUninterruptedCastWarning;
    private int ChantLineIndex;
    private int SpawnAddsCount;
    private bool InvertedRoomAoe;
    private bool ShowedSlowRoomAoeAnimation;
    private int RoomAoeAnimationIndex1;
    private int RoomAoeAnimationIndex2;
    private List<Point> RoomAoeAnimationPoints;
    
    #region Services
    private readonly IMonsterFactory MonsterFactory;
    private readonly IReactorTileFactory ReactorTileFactory;
    private readonly IEffectFactory EffectFactory;
    #endregion
    
    #region Timers
    private readonly IIntervalTimer TeleportTimer = new RandomizedIntervalTimer(
        TimeSpan.FromSeconds(15),
        15,
        RandomizationType.Positive,
        false);
    private readonly IIntervalTimer ReignOfFireTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly IIntervalTimer TeleportPhaseTimer = new IntervalTimer(TimeSpan.FromMinutes(1), false);
    private readonly IIntervalTimer ChantTimer = new IntervalTimer(TimeSpan.FromMilliseconds(2000), false);
    private readonly IIntervalTimer AfterChantTimer1 = new IntervalTimer(TimeSpan.FromSeconds(5), false);
    private readonly IIntervalTimer AfterChantTimer2 = new IntervalTimer(TimeSpan.FromSeconds(5), false);
    private readonly IIntervalTimer SpawnAddsTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    private readonly IIntervalTimer BurningGroundTimer = new IntervalTimer(TimeSpan.FromSeconds(20), false);
    private readonly IIntervalTimer TurnTimer = new IntervalTimer(TimeSpan.FromSeconds(3), false);
    private readonly IIntervalTimer SafePointTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
    private readonly IIntervalTimer RoomAoeAnimationTimer = new IntervalTimer(TimeSpan.FromMilliseconds(200));
    private readonly SequentialEventTimer RoomeAoePhaseTimer;
    #endregion
    
    #region Spells
    private readonly Spell BlazingNova;
    private readonly Spell FocusedDestruction;
    private readonly Spell ReignOfFire;
    private readonly Spell Cataclysm;
    private readonly Spell ReverseCataclysm;
    #endregion

    /// <inheritdoc />
    public ShamensythScript(
        Monster subject,
        ISpellFactory spellFactory,
        IMonsterFactory monsterFactory,
        IReactorTileFactory reactorTileFactory,
        IEffectFactory effectFactory)
        : base(subject)
    {
        MonsterFactory = monsterFactory;
        ReactorTileFactory = reactorTileFactory;
        EffectFactory = effectFactory;
        FocusedDestruction = spellFactory.Create("sham_focusedDestruction");
        BlazingNova = spellFactory.Create("sham_blazingNova");
        ReignOfFire = spellFactory.Create("sham_reignoffire");
        Cataclysm = spellFactory.Create("sham_cataclysm");
        ReverseCataclysm = spellFactory.Create("sham_reverseCataclysm");

        RoomeAoePhaseTimer = new SequentialEventTimer(
            Enumerable.Repeat(ChantTimer, ChantLines.Count)
                      .Append(AfterChantTimer1)
                      .Append(AfterChantTimer2)
                      .ToList());
        
        RoomAoeAnimationPoints = new Circle(RoomAoePoint, 3).GetOrderedOutline()
                                                            .ToList();

        var roomPoints = Subject.MapInstance
                                .Template
                                .Bounds
                                .GetPoints()
                                .ToList();

        var unsafePoints = roomPoints.FilterByLineOfSight(RoomAoePoint, Subject.MapInstance);

        SafePoints = roomPoints.Except(unsafePoints)
                               .Where(pt => !Subject.MapInstance.IsWall(pt))
                               .ToList();

        var wallsInAoeRange = roomPoints.Where(pt => pt.WithinRange(RoomAoePoint, 12))
                                        .Where(pt => Subject.MapInstance.IsWall(pt));

        InvertedSafePoints = wallsInAoeRange.SelectMany(pt => pt.RayTraceTo(RoomAoePoint))
                                            .Where(pt => !Subject.MapInstance.IsWall(pt))
                                            .Where(pt => !RoomAoePoint.WithinRange(pt, 1))
                                            .ToList();
        
        SetRoomAoeAnimationPoints();
    }
    
    private void SetRoomAoeAnimationPoints()
    {
        RoomAoeAnimationIndex1 = 0;
        RoomAoeAnimationIndex2 = RoomAoeAnimationPoints.Count / 2;
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

            //add 10 burning ground tiles on spots not already occupied by burning ground
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
                case 6:
                {
                    var addsAlive = Map.GetEntities<Monster>()
                                       .Any(monster => monster.Template.TemplateKey.EqualsI("shamensythFireElemental"));
                    
                    if(addsAlive)
                        ApplyInvulnerability();
                    else
                        RemoveInvulnerability();
                    
                    break;
                }
            }
    }

    private void ApplyInvulnerability()
    {
        if (!Subject.Effects.Contains("invulnerability"))
        {
            var effect = EffectFactory.Create("invulnerability");
            Subject.Effects.Apply(Subject, effect);
        }
    }

    private void RemoveInvulnerability() => Subject.Effects.Dispel("invulnerability");

    private void AnimateSafePoints()
    {
        //only animate safe points during chant timer and AfterChantTimer1
        if ((RoomeAoePhaseTimer.CurrentTimer != AfterChantTimer2) && SafePointTimer.IntervalElapsed)
            foreach (var point in SafePoints)
                Subject.MapInstance.ShowAnimation(SafePointAnimation.GetPointAnimation(point));
    }

    private void AnimateInvertedSafePoints()
    {
        //only animate safe points during chant timer and AfterChantTimer1
        if ((RoomeAoePhaseTimer.CurrentTimer != AfterChantTimer2) && SafePointTimer.IntervalElapsed)
            foreach (var point in InvertedSafePoints)
                Subject.MapInstance.ShowAnimation(SafePointAnimation.GetPointAnimation(point));
    }
    
    private void HandleRoomAoe()
    {
        if (RoomeAoePhaseTimer.CurrentTimer == ChantTimer)
        {
            ApplyInvulnerability();
            HandleRoomAoeAnimation();
        }

        if (!InvertedRoomAoe)
        {
            AnimateSafePoints();
            
            if (RoomeAoePhaseTimer.IntervalElapsed)
                if (RoomeAoePhaseTimer.CurrentTimer == ChantTimer)
                {
                    if (!ShowedSlowRoomAoeAnimation)
                    {
                        //slow animation plays once
                        Subject.MapInstance.ShowAnimation(RoomAoeAnimation1.GetPointAnimation(Subject));
                        Subject.MapInstance.ShowAnimation(RoomAoeAnimation2.GetPointAnimation(Subject));
                        ShowedSlowRoomAoeAnimation = true;
                    }
                    
                    if (ChantLineIndex < ChantLines.Count)
                    {
                        Subject.Shout(ChantLines[ChantLineIndex]);
                        ChantLineIndex++;

                        if (ChantLineIndex == ChantLines.Count)
                            if(Subject.TryUseSpell(Cataclysm))
                                Subject.AnimateBody(BodyAnimation.Assail, 70);
                    }
                } else if (RoomeAoePhaseTimer.CurrentTimer == AfterChantTimer1)
                    RemoveInvulnerability();
                else if (RoomeAoePhaseTimer.CurrentTimer == AfterChantTimer2)
                {
                    CurrentPhase = Phase.Teleport;
                    ChantLineIndex = 0;
                    SetRoomAoeAnimationPoints();
                    ShowedSlowRoomAoeAnimation = false;
                    RoomeAoePhaseTimer.Reset();
                }
        } else
        {
            AnimateInvertedSafePoints();
            
            if (RoomeAoePhaseTimer.IntervalElapsed)
                if (RoomeAoePhaseTimer.CurrentTimer == ChantTimer)
                {
                    if (!ShowedSlowRoomAoeAnimation)
                    {
                        //slow animation plays once
                        Subject.MapInstance.ShowAnimation(RoomAoeAnimation1.GetPointAnimation(Subject));
                        Subject.MapInstance.ShowAnimation(RoomAoeAnimation2.GetPointAnimation(Subject));
                        ShowedSlowRoomAoeAnimation = true;
                    }
                    
                    if (ChantLineIndex < ChantLines.Count)
                    {
                        Subject.Shout(new string(ChantLines[ChantLines.Count - ChantLineIndex - 1]));
                        ChantLineIndex++;

                        if (ChantLineIndex == ChantLines.Count)
                            if(Subject.TryUseSpell(ReverseCataclysm))
                                Subject.AnimateBody(BodyAnimation.Assail, 50);
                    }
                } else if (RoomeAoePhaseTimer.CurrentTimer == AfterChantTimer1)
                    RemoveInvulnerability();
                else if (RoomeAoePhaseTimer.CurrentTimer == AfterChantTimer2)
                {
                    CurrentPhase = Phase.Teleport;
                    ChantLineIndex = 0;
                    SetRoomAoeAnimationPoints();
                    ShowedSlowRoomAoeAnimation = false;
                    RoomeAoePhaseTimer.Reset();
                }
        }
    }

    private void HandleReignOfFire()
    {
        var nearbyAisling = Subject.MapInstance
                                   .GetEntitiesWithinRange<Aisling>(Subject, 1)
                                   .ThatAreAlive()
                                   .ThatAreNotInGodMode();

        if (!nearbyAisling.Any())
        {
            if (!GaveUninterruptedCastWarning)
            {
                foreach (var aisling in Subject.MapInstance.GetEntities<Aisling>())
                    aisling.SendActiveMessage("The shamensyth is able to cast without interruption!");

                GaveUninterruptedCastWarning = true;
            }

            if (Subject.TryUseSpell(ReignOfFire))
                Subject.AnimateBody(BodyAnimation.Assail, 50);
        }
    }

    private void HandleFacing()
    {
        if (CurrentPhase == Phase.Teleport)
        {
            if (TurnTimer.IntervalElapsed && Subject.Target is not null)
            {
                var direction = Subject.Target.DirectionalRelationTo(Subject);
                
                if (direction != Direction.Invalid)
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

        if (Subject.Target is not null)
        {
            var direction = Subject.Target.DirectionalRelationTo(Subject);

            if (direction != Direction.Invalid)
                Subject.Turn(direction);
        }

        ReignOfFireTimer.Reset();
    }
    
    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        //timer updates
        SpawnAddsTimer.Update(delta);
        BurningGroundTimer.Update(delta);
        TurnTimer.Update(delta);
        SafePointTimer.Update(delta);
        
        //spell updates
        BlazingNova.Update(delta);
        FocusedDestruction.Update(delta);
        
        //do things independent of phase
        HandleFacing();
        HandleAdds();
        HandleBurningGround();
        
        //handle phases
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

                //basically his autoattacks
                if (Subject.SpellTimer.IntervalElapsed)
                {
                    if (Subject.Target is not null)
                        Subject.TryUseSpell(FocusedDestruction, Subject.Target.Id);

                    var surroundingPoints = Subject.SpiralSearch(2);
                    var aislingIsNearby = Map.GetEntitiesAtPoints<Aisling>(surroundingPoints)
                                             .Any();

                    if (aislingIsNearby)
                        Subject.TryUseSpell(BlazingNova);
                }

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
                RoomAoeAnimationTimer.Update(delta);

                if (!RoomAoePoint.Equals(Subject))
                    Subject.WarpTo(RoomAoePoint);

                RoomeAoePhaseTimer.Update(delta);
                HandleRoomAoe();
                
                break;
            }
        }
    }

    private void HandleRoomAoeAnimation()
    {
        if (RoomAoeAnimationTimer.IntervalElapsed)
        {
            if (RoomAoeAnimationIndex1 >= RoomAoeAnimationPoints.Count)
                RoomAoeAnimationIndex1 = 0;

            if (RoomAoeAnimationIndex2 >= RoomAoeAnimationPoints.Count)
                RoomAoeAnimationIndex2 = 0;

            var point1 = RoomAoeAnimationPoints[RoomAoeAnimationIndex1++];
            var point2 = RoomAoeAnimationPoints[RoomAoeAnimationIndex2++];

            Subject.MapInstance.ShowAnimation(RoomAoeAnimation3.GetPointAnimation(point1));
            Subject.MapInstance.ShowAnimation(RoomAoeAnimation3.GetPointAnimation(point2));
        }
    }
}