using Chaos.Common.Utilities;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.Creants.Tauren;

public sealed class TaurenPhaseScript : MonsterScriptBase
{
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IntervalTimer ChannelDurationTimer;
    private readonly IntervalTimer ChannelIntervalTimer;
    private readonly IntervalTimer DelayTimer;
    private readonly IMonsterFactory MonsterFactory;
    private readonly IIntervalTimer PhaseProcessTimer;
    private readonly IIntervalTimer PhaseTimer;
    private readonly IntervalTimer SafePointAnimationTimer;
    private readonly List<Point> SafePoints;
    private readonly ISkillFactory SkillFactory;
    private readonly IntervalTimer SkillTimer;
    private readonly IntervalTimer SpellDelay;
    private readonly ISpellFactory SpellFactory;
    private readonly IntervalTimer SpellTimer;

    private readonly IIntervalTimer SplitPhaseTimer;
    private bool IsChanneling;
    private bool Spell1;
    private bool Spell2;

    private int SpellCasts1;
    private int Spin;

    private bool SplitPhase1;
    private int SummonedTaurens;
    private bool TurnedClockwise;

    private int CurrentPhase { get; set; } // Tracks the current phase

    /// <inheritdoc />
    public TaurenPhaseScript(
        Monster subject,
        IMonsterFactory monsterFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory)
        : base(subject)
    {
        ChannelIntervalTimer = new IntervalTimer(TimeSpan.FromSeconds(15));
        SkillTimer = new IntervalTimer(TimeSpan.FromMilliseconds(500));
        SpellTimer = new IntervalTimer(TimeSpan.FromSeconds(20));
        SpellDelay = new IntervalTimer(TimeSpan.FromMilliseconds(200));
        PhaseTimer = new IntervalTimer(TimeSpan.FromSeconds(20));
        ChannelDurationTimer = new IntervalTimer(TimeSpan.FromSeconds(10));
        SafePointAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        DelayTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1250), false);
        PhaseProcessTimer = new IntervalTimer(TimeSpan.FromMinutes(1), false);
        SafePoints = new List<Point>();
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        MonsterFactory = monsterFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
        SplitPhaseTimer = new IntervalTimer(TimeSpan.FromSeconds(10), false);
    }

    #region SkillPhase
    private void SkillPhase(TimeSpan delta)
    {
        SkillTimer.Update(delta);
        DelayTimer.Update(delta);
        PhaseProcessTimer.Update(delta);

        var skill1 = SkillFactory.Create("tauren_coneattack");

        if (PhaseProcessTimer.IntervalElapsed)
            ResetPhase();

        if ((Spin >= 1) && DelayTimer.IntervalElapsed)
        {
            Spin = 0;

            if (Subject.Direction == Direction.Up)
            {
                Subject.Turn(Direction.Right);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Right)
            {
                Subject.Turn(Direction.Down);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Down)
            {
                Subject.Turn(Direction.Left);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Left)
            {
                Subject.Turn(Direction.Up);
                Subject.TryUseSkill(skill1);

                return;
            }
        }

        if (DelayTimer.IntervalElapsed && TurnedClockwise)
        {
            TurnedClockwise = false;
            Spin++;

            if (Subject.Direction == Direction.Right)
            {
                Subject.Turn(Direction.Up);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Up)
            {
                Subject.Turn(Direction.Left);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Left)
            {
                Subject.Turn(Direction.Down);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Down)
            {
                Subject.Turn(Direction.Right);
                Subject.TryUseSkill(skill1);

                return;
            }
        }

        if (DelayTimer.IntervalElapsed && !TurnedClockwise)
        {
            TurnedClockwise = true;

            if (Subject.Direction == Direction.Up)
            {
                Subject.Turn(Direction.Right);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Right)
            {
                Subject.Turn(Direction.Down);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Down)
            {
                Subject.Turn(Direction.Left);
                Subject.TryUseSkill(skill1);

                return;
            }

            if (Subject.Direction == Direction.Left)
            {
                Subject.Turn(Direction.Up);
                Subject.TryUseSkill(skill1);
            }
        }
    }
    #endregion

    #region SpellPhase
    private void SpellPhase(TimeSpan delta)
    {
        SpellTimer.Update(delta);
        SpellDelay.Update(delta);
        PhaseProcessTimer.Update(delta);

        if (SpellTimer.IntervalElapsed)
        {
            if (!Spell1)
            {
                var spell1 = SpellFactory.Create("tauren_freezespell1");
                Subject.TryUseSpell(spell1);
                Subject.TryUseSpell(spell1);
                Spell1 = true;
            }

            if (!Spell2 && Spell1)
            {
                var spell2 = SpellFactory.Create("tauren_freezespell2");
                Subject.TryUseSpell(spell2);
                Spell2 = true;
            }
        }

        if (Spell1 && !Spell2 && SpellDelay.IntervalElapsed)
        {
            var spell1 = SpellFactory.Create("tauren_freezespell1");

            SpellCasts1++;

            if (SpellCasts1 <= 6)
                Subject.TryUseSpell(spell1);
        }

        if (PhaseProcessTimer.IntervalElapsed)
            ResetPhase();
    }
    #endregion

    #region SplitPhase
    private void SplitPhase(TimeSpan delta)
    {
        SplitPhaseTimer.Update(delta);
        PhaseProcessTimer.Update(delta);

        if (!SplitPhase1)
        {
            SplitPhase1 = true;

            var aislings = Subject.MapInstance.GetEntities<Aisling>();

            foreach (var aisling in aislings)
                aisling.Trackers.Enums.Set(CreantPhases.None);

            for (var i = SummonedTaurens; i < 4; i++)
            {
                var options = new AoeShapeOptions
                {
                    Source = new Point(Subject.X, Subject.Y),
                    Range = 5
                };

                var points = AoeShape.AllAround.ResolvePoints(options);

                var validPoint = points.Where(validPoints => Subject.MapInstance.IsWalkable(validPoints, CreatureType.Normal))
                                       .ToList();
                var point = validPoint.PickRandom();

                var summon = MonsterFactory.Create("TaurenSpawn", Subject.MapInstance, point);
                Subject.MapInstance.AddEntity(summon, point);
            }

            var grabSummon = Subject.MapInstance
                                    .GetEntities<Monster>()
                                    .Where(x => x.Template.TemplateKey == "TaurenSpawn")
                                    .ToList();

            var switchSummon = grabSummon.PickRandom();

            var taurenPoint = new Point(Subject.X, Subject.Y);

            Subject.WarpTo(switchSummon);
            switchSummon.WarpTo(taurenPoint);
        }

        if (SplitPhaseTimer.IntervalElapsed)
        {
            var grabSummon = Subject.MapInstance
                                    .GetEntities<Monster>()
                                    .Where(x => x.Template.TemplateKey == "TaurenSpawn")
                                    .ToList();

            if (grabSummon.Count == 0)
                return;

            var switchSummon = grabSummon.PickRandom();

            var summonDirectionFacing = switchSummon.Direction;
            var taurenDirectionFacing = Subject.Direction;

            var taurenPoint = new Point(Subject.X, Subject.Y);

            Subject.WarpTo(switchSummon);
            Subject.Turn(summonDirectionFacing);
            switchSummon.WarpTo(taurenPoint);
            switchSummon.Turn(taurenDirectionFacing);
        }

        if (PhaseProcessTimer.IntervalElapsed)
        {
            var spawnedTaurens = Subject.MapInstance
                                        .GetEntities<Monster>()
                                        .Where(x => x.Template.TemplateKey == "TaurenSpawn")
                                        .ToList();

            if (spawnedTaurens.Count != 0)
            {
                Subject.Say("They are no match for us.");

                foreach (var spawn in spawnedTaurens)
                {
                    Subject.MapInstance.RemoveEntity(spawn);

                    if (Subject.StatSheet.MaximumHp > Subject.StatSheet.CurrentHp)
                        Subject.StatSheet.AddHealthPct(5);
                }
            }

            ResetPhase();
        }
    }
    #endregion

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        PhaseTimer.Update(delta);

        // Change phases every minute
        if (PhaseTimer.IntervalElapsed && (CurrentPhase == 0))
        {
            var roll = IntegerRandomizer.RollSingle(1);
            CurrentPhase = 3;

            HandlePhaseChange();
        }

        // Execute mechanics based on the current phase
        switch (CurrentPhase)
        {
            case 1:
                SplitPhase(delta);

                break;
            case 2:
                RoomNukePhase(delta);

                break;
            case 3:
                SkillPhase(delta);

                break;
            case 4:
                SpellPhase(delta);

                break;
        }
    }

    #region NukeRoomPhase
    private void AnimateSafePoints()
    {
        Subject.MapInstance.ShowAnimation(
            new Animation
            {
                AnimationSpeed = 100,
                TargetAnimation = 840,
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

    private void CreateSafePoints()
    {
        var safePoints = new[]
        {
            new Point(6, 8),
            new Point(7, 8),
            new Point(6, 9),
            new Point(7, 9),
            new Point(7, 10),
            new Point(8, 9),
            new Point(8, 10),
            new Point(6, 16),
            new Point(6, 15),
            new Point(7, 16),
            new Point(7, 15),
            new Point(7, 14),
            new Point(8, 15),
            new Point(8, 14),
            new Point(12, 14),
            new Point(12, 15),
            new Point(13, 14),
            new Point(13, 15),
            new Point(13, 16),
            new Point(14, 15),
            new Point(14, 16),
            new Point(12, 10),
            new Point(13, 10),
            new Point(12, 9),
            new Point(13, 9),
            new Point(13, 8),
            new Point(14, 9),
            new Point(14, 8)
        };

        foreach (var point in safePoints)
            SafePoints.Add(point);
    }

    private void DamagePlayersNotOnSafePoints()
    {
        foreach (var player in Subject.MapInstance.GetEntities<Aisling>())
        {
            if (SafePoints.Any(p => (p.X == player.X) && (p.Y == player.Y)))
                continue;

            var damage = (int)(player.StatSheet.EffectiveMaximumHp * 0.95);

            ApplyDamageScript.ApplyDamage(
                Subject,
                player,
                this,
                damage);
        }
    }

    private void EndChanneling()
    {
        IsChanneling = false;
        SafePointAnimationTimer.Reset();
        DamagePlayersNotOnSafePoints();
        ShowFinalAnimations();
        SafePoints.Clear();
    }

    private void RoomNukePhase(TimeSpan delta)
    {
        ChannelIntervalTimer.Update(delta);
        SafePointAnimationTimer.Update(delta);
        PhaseProcessTimer.Update(delta);

        if (IsChanneling)
        {
            ChannelDurationTimer.Update(delta);

            if (SafePointAnimationTimer.IntervalElapsed)
                AnimateSafePoints();

            if (ChannelDurationTimer.IntervalElapsed)
                EndChanneling();
        } else if (ChannelIntervalTimer.IntervalElapsed)
            StartChanneling();

        if (PhaseProcessTimer.IntervalElapsed)
            ResetPhase();
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
                            TargetAnimation = 990,
                            TargetPoint = point
                        });
            }
        }
    }

    private void StartChanneling()
    {
        IsChanneling = true;
        ChannelDurationTimer.Reset();
        ChannelIntervalTimer.Reset();
        SafePointAnimationTimer.Reset();

        CreateSafePoints();
    }
    #endregion

    #region PhaseHandling
    private void HandlePhaseChange()
    {
        // Add phase-specific effects or animations here
        switch (CurrentPhase)
        {
            case 1:
                StartPhase();

                break;
            case 2:
                StartPhase();

                // Add a new effect or animation for Phase 2
                break;
            case 3:
                StartPhase();

                // Add a new effect or animation for Phase 3
                break;
            case 4:
                StartPhase();

                // Add a new effect or animation for Phase 4
                break;
        }
    }

    private void StartPhase()
    {
        var aislings = Subject.MapInstance.GetEntities<Aisling>();

        foreach (var aisling in aislings)
            aisling.Trackers.Enums.Set(CreantPhases.InPhase);

        var point = new Point(10, 12);
        var direction = Direction.Down;
        Subject.WarpTo(point);
        Subject.Turn(direction);

        switch (CurrentPhase)
        {
            case 1:
                Subject.Say("This is my Domain.");

                break;
            case 2:
                Subject.Say("You all could have left me alone.");

                break;
            case 3:
                Subject.Say("Understand my troubles Aislings.");

                break;
            case 4:
                Subject.Say("Leave my cave at once.");

                break;
        }
    }

    private void ResetPhase()
    {
        IsChanneling = false;
        SplitPhase1 = false;
        Spell1 = false;
        Spell2 = false;
        TurnedClockwise = false;
        SpellCasts1 = 0;
        SummonedTaurens = 0;
        CurrentPhase = 0;
        Spin = 0;

        var aislings = Subject.MapInstance.GetEntities<Aisling>();

        foreach (var aisling in aislings)
            aisling.Trackers.Enums.Set(CreantPhases.None);
    }
    #endregion
}