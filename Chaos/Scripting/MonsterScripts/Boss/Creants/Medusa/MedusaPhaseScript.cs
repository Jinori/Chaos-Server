using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Medusa;

public sealed class MedusaPhaseScript : MonsterScriptBase
{
    private readonly IntervalTimer ChannelIntervalTimer;
    private readonly IntervalTimer DelayTimer;
    private readonly IEffectFactory EffectFactory;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IntervalTimer NukeRoomTimer;
    private readonly IntervalTimer PhaseDelay;
    private readonly IntervalTimer SafePointAnimationTimer;

    private readonly List<Point> SafePoints;
    private readonly IntervalTimer SpawnPhaseTimer;
    private readonly IntervalTimer SpawnSnakePitsTimer;
    private readonly ISpellFactory SpellFactory;
    private readonly IIntervalTimer SpellPhaseTimer4;
    private readonly IntervalTimer SpellTimer;
    private readonly IntervalTimer StoneChannelingTimer;
    private readonly IIntervalTimer TimeBetweenPhases;

    private bool CreatedSafePoints;

    public bool InPhase;
    private bool IsChanneling;

    private bool Spell1;
    private bool Spell2;
    private bool Spell3;

    private bool StartSpellPhase;
    private bool StoneReady;

    private int CurrentPhase { get; set; } // Tracks the current phase

    /// <inheritdoc />
    public MedusaPhaseScript(
        Monster subject,
        IMonsterFactory monsterFactory,
        ISpellFactory spellFactory,
        IEffectFactory effectFactory)
        : base(subject)
    {
        StoneChannelingTimer = new IntervalTimer(TimeSpan.FromSeconds(15), false);
        SpawnPhaseTimer = new IntervalTimer(TimeSpan.FromSeconds(45), false);
        SpellTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);
        PhaseDelay = new IntervalTimer(TimeSpan.FromSeconds(2), false);
        TimeBetweenPhases = new IntervalTimer(TimeSpan.FromSeconds(60), false);
        SpellPhaseTimer4 = new IntervalTimer(TimeSpan.FromSeconds(12), false);
        DelayTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1250), false);
        SpawnSnakePitsTimer = new IntervalTimer(TimeSpan.FromSeconds(12), false);
        MonsterFactory = monsterFactory;
        SpellFactory = spellFactory;
        EffectFactory = effectFactory;
        SafePoints = new List<Point>();

        SafePointAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        ChannelIntervalTimer = new IntervalTimer(TimeSpan.FromSeconds(12), false);
        NukeRoomTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
    }

    #region NukeRoomPhase
    private void SpawnSnakePits(TimeSpan delta)
    {
        SpawnSnakePitsTimer.Update(delta);
        SpawnPhaseTimer.Update(delta);
        PhaseDelay.Update(delta);

        if (SpawnPhaseTimer.IntervalElapsed)
            ResetPhase();

        if (SpawnSnakePitsTimer.IntervalElapsed)
        {
            var snakepits = Subject.MapInstance
                                   .GetEntities<Monster>()
                                   .Where(x => x.Name == "Snake Pit")
                                   .ToList();

            if (snakepits.Count > 7)
            {
                ResetPhase();

                return;
            }

            var rectangle = new Rectangle(
                Subject.X - 6,
                Subject.Y - 6,
                15,
                15);

            const int MAX_RETRIES = 20; // Limit retries to prevent infinite loop
            Point? spawnPoint = null;

            for (var i = 0; i < MAX_RETRIES; i++)
            {
                var candidatePoint = rectangle.GetRandomPoint();

                if (Subject.MapInstance.IsWalkable(candidatePoint, CreatureType.Normal))
                {
                    spawnPoint = candidatePoint;

                    break;
                }
            }

            if (spawnPoint.HasValue)
            {
                var snakepit = MonsterFactory.Create("snakepit", Subject.MapInstance, spawnPoint.Value);
                Subject.MapInstance.AddEntity(snakepit, spawnPoint.Value);
            }
        }
    }
    #endregion

    #region SpellPhase
    private void SpellPhase(TimeSpan delta)
    {
        SpellTimer.Update(delta);
        SpellPhaseTimer4.Update(delta);

        var spell1 = SpellFactory.Create("medusa_wave1");

        if (SpellPhaseTimer4.IntervalElapsed)
        {
            var point = new Point(11, 22);
            var direction = Direction.Down;
            Subject.WarpTo(point);
            Subject.Turn(direction);
            ResetPhase();
        }

        if (PhaseDelay.IntervalElapsed)
            StartSpellPhase = true;

        if (!Spell1 && SpellTimer.IntervalElapsed && StartSpellPhase)
        {
            var point = new Point(11, 29);
            Subject.WarpTo(point);
            Subject.Turn(Direction.Up);
            Subject.TryUseSpell(spell1);
            Spell1 = true;

            return;
        }

        if (SpellTimer.IntervalElapsed && !Spell2 && Spell1)
        {
            var point = new Point(11, 16);
            Subject.WarpTo(point);
            Subject.Turn(Direction.Down);
            Spell2 = true;
            Subject.TryUseSpell(spell1);

            return;
        }

        if (!Spell3 && Spell2 && SpellTimer.IntervalElapsed)
        {
            var point = new Point(5, 22);
            Subject.WarpTo(point);
            Subject.Turn(Direction.Right);
            Spell3 = true;
            Subject.TryUseSpell(spell1);

            return;
        }

        if (Spell3 && Spell2 && Spell1 && SpellTimer.IntervalElapsed)
        {
            var point = new Point(17, 22);
            Subject.WarpTo(point);
            Subject.Turn(Direction.Left);
            Spell3 = false;
            Spell2 = false;
            Spell1 = false;
            Subject.TryUseSpell(spell1);
        }
    }
    #endregion

    #region StonePhase
    private void StonePhase(TimeSpan delta)
    {
        StoneChannelingTimer.Update(delta);
        DelayTimer.Update(delta);

        if (!StoneReady && StoneChannelingTimer.IntervalElapsed)
        {
            var ani2 = new Animation
            {
                AnimationSpeed = 200,
                TargetAnimation = 483
            };
            Subject.MapInstance.ShowAnimation(ani2);
            StoneReady = true;
        }

        if (!StoneReady && DelayTimer.IntervalElapsed)
        {
            var ani = new Animation
            {
                AnimationSpeed = 200,
                TargetAnimation = 490
            };

            Subject.MapInstance.ShowAnimation(ani);
        }

        if (DelayTimer.IntervalElapsed && StoneReady)
        {
            var aislings = Subject.MapInstance
                                  .GetEntities<Aisling>()
                                  .Where(x => x.IsAlive)
                                  .ToList();

            var ani3 = new Animation
            {
                AnimationSpeed = 200,
                TargetAnimation = 85
            };

            foreach (var aisling in aislings)
            {
                var direction = aisling.Direction;
                var subjectDirection = Subject.Direction;

                if ((subjectDirection == Direction.Down) && (direction == Direction.Up) && (Subject.Y < aisling.Y))
                {
                    aisling.Animate(ani3);
                    var stoned = EffectFactory.Create("Stoned");
                    aisling.Effects.Apply(Subject, stoned);
                    aisling.SendOrangeBarMessage("Your body starts to turn to stone!");
                }

                if ((subjectDirection == Direction.Up) && (direction == Direction.Down) && (Subject.Y > aisling.Y))
                {
                    aisling.Animate(ani3);
                    var stoned = EffectFactory.Create("Stoned");
                    aisling.Effects.Apply(Subject, stoned);
                    aisling.SendOrangeBarMessage("Your body starts to turn to stone!");
                }

                if ((subjectDirection == Direction.Right) && (direction == Direction.Left) && (Subject.X < aisling.X))
                {
                    aisling.Animate(ani3);
                    var stoned = EffectFactory.Create("Stoned");
                    aisling.Effects.Apply(Subject, stoned);
                    aisling.SendOrangeBarMessage("Your body starts to turn to stone!");
                }

                if ((subjectDirection == Direction.Left) && (direction == Direction.Right) && (Subject.X > aisling.X))
                {
                    aisling.Animate(ani3);
                    var stoned = EffectFactory.Create("Stoned");
                    aisling.Effects.Apply(Subject, stoned);
                    aisling.SendOrangeBarMessage("Your body starts to turn to stone!");
                }
            }

            Subject.Say("May stone will fill your mortal body.");
            StoneReady = false;
        }
    }
    #endregion

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        PhaseDelay.Update(delta);
        TimeBetweenPhases.Update(delta);

        if (!InPhase)
            StonePhase(delta);

        if (TimeBetweenPhases.IntervalElapsed && !InPhase)
        {
            CurrentPhase++;
            InPhase = true;

            if (CurrentPhase > 3)
                CurrentPhase = 1;

            StartPhase();
        }

        if (InPhase)
            switch (CurrentPhase)
            {
                case 1:
                    SpellPhase(delta);

                    break;
                case 2:
                    FloodRoom(delta);

                    break;
                case 3:
                    SpawnSnakePits(delta);

                    break;
            }

        // Execute mechanics based on the current phase
    }

    #region NukeRoomPhase
    private void AnimateSafePoints()
    {
        Subject.MapInstance.ShowAnimation(
            new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 511,
                TargetPoint = new Point(Subject.X, Subject.Y)
            });

        foreach (var point in SafePoints)
            Subject.MapInstance.ShowAnimation(
                new Animation
                {
                    AnimationSpeed = 100,
                    TargetAnimation = 214,
                    TargetPoint = point
                });
    }

    private void DamagePlayersNotOnSafePoints()
    {
        foreach (var player in Subject.MapInstance.GetEntities<Aisling>())
        {
            if (SafePoints.Any(p => (p.X == player.X) && (p.Y == player.Y)))
                continue;

            var drowning = EffectFactory.Create("Drowning");
            player.Effects.Apply(Subject, drowning);
        }
    }

    private void FloodRoom(TimeSpan delta)
    {
        // Update timers for phase control
        ChannelIntervalTimer.Update(delta);
        SafePointAnimationTimer.Update(delta);
        NukeRoomTimer.Update(delta);

        // Start the channeling phase when the interval elapses
        if (ChannelIntervalTimer.IntervalElapsed && !IsChanneling)
            IsChanneling = true;

        // Ensure safe points are created during the first pass
        if (!CreatedSafePoints)
        {
            CreateSafePoints();
            CreatedSafePoints = true;

            var aislings = Subject.MapInstance
                                  .GetEntities<Aisling>()
                                  .Where(x => x.IsAlive)
                                  .ToList();

            foreach (var aisling in aislings)
                aisling.SendOrangeBarMessage("The room is filling up with water! Get to higher ground.");
        }

        // Handle Room Nuke repetitions
        if (IsChanneling)
            if (NukeRoomTimer.IntervalElapsed)
            {
                // Apply damage and show animations for the nuke
                ShowFinalAnimations();
                DamagePlayersNotOnSafePoints();
                ResetPhase();
            }

        // Show safe point animations periodically
        if (SafePointAnimationTimer.IntervalElapsed)
            AnimateSafePoints();
    }

    private void CreateSafePoints()
    {
        // Define the rectangle boundaries
        var rectangle = new Rectangle(
            9,
            7,
            8,
            8);

        var points = rectangle.GetPoints();

        foreach (var point in points)
            if (!Subject.MapInstance.IsWall(point))

                // Add the point to SafePoints
                SafePoints.Add(point);
    }

    private void ShowFinalAnimations()
    {
        for (var x = 0; x < Subject.MapInstance.Template.Width; x++)
        {
            for (var y = 0; y < Subject.MapInstance.Template.Height; y++)
            {
                var point = new Point(x, y);

                if (!SafePoints.Contains(point) && !Subject.MapInstance.IsWall(point) && !Subject.MapInstance.IsBlockingReactor(point))
                    Subject.MapInstance.ShowAnimation(
                        new Animation
                        {
                            AnimationSpeed = 100,
                            TargetAnimation = 222,
                            TargetPoint = point
                        });
            }
        }
    }
    #endregion

    #region PhaseHandling
    private void StartPhase()
    {
        var point = new Point(11, 22);
        var direction = Direction.Down;
        Subject.WarpTo(point);
        Subject.Turn(direction);

        switch (CurrentPhase)
        {
            case 1:
                Subject.Say("May the oceans swallow you whole.");

                break;
            case 2:
                Subject.Say("I am all around you.");

                break;
            case 3:
                Subject.Say("Become stiff Aisling.");

                break;
        }

        PhaseDelay.Reset();
    }

    private void ResetPhase()
    {
        Spell1 = false;
        Spell2 = false;
        Spell3 = false;
        StartSpellPhase = false;
        CreatedSafePoints = false;
        IsChanneling = false;
        TimeBetweenPhases.Reset();
        InPhase = false;
        PhaseDelay.Reset();
        SafePoints.Clear();
    }
    #endregion
}