using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;

namespace Chaos.Scripting.MonsterScripts.Boss;

public sealed class BossChannelNukeScript : MonsterScriptBase
{
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IntervalTimer ChannelDurationTimer;
    private readonly IntervalTimer ChannelIntervalTimer;
    private readonly IntervalTimer SafePointAnimationTimer;
    private readonly List<Point> SafePoints;
    private bool IsChanneling;

    public BossChannelNukeScript(Monster subject)
        : base(subject)
    {
        ChannelIntervalTimer = new IntervalTimer(TimeSpan.FromSeconds(35));
        ChannelDurationTimer = new IntervalTimer(TimeSpan.FromSeconds(5));
        SafePointAnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1));
        SafePoints = new List<Point>();
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
    }

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
        var randomPoint = GetRandomSafeAreaCenter();

        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                var point = new Point(randomPoint.X + x, randomPoint.Y + y);
                SafePoints.Add(point);
            }
        }
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

    private Point GetRandomSafeAreaCenter()
    {
        const int MAX_ATTEMPTS = 100;
        var attempt = 0;

        while (attempt < MAX_ATTEMPTS)
        {
            var randomX = Random.Shared.Next(Subject.MapInstance.Template.Width);
            var randomY = Random.Shared.Next(Subject.MapInstance.Template.Height);
            var point = new Point(randomX, randomY);

            if (IsSafeArea(point))
                return point;

            attempt++;
        }

        return new Point(0, 0);
    }

    private bool IsSafeArea(Point center)
    {
        for (var x = -1; x <= 1; x++)
        {
            for (var y = -1; y <= 1; y++)
            {
                var point = new Point(center.X + x, center.Y + y);

                if (Subject.MapInstance.IsWall(point) || Subject.MapInstance.IsBlockingReactor(point))
                    return false;
            }
        }

        return true;
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

        foreach (var player in Subject.MapInstance.GetEntities<Aisling>())
            player.SendActiveMessage($"{Subject.Name} is channeling a spell, get to safety!");
        CreateSafePoints();
    }

    public override void Update(TimeSpan delta)
    {
        ChannelIntervalTimer.Update(delta);
        SafePointAnimationTimer.Update(delta);

        if (IsChanneling)
        {
            ChannelDurationTimer.Update(delta);

            if (SafePointAnimationTimer.IntervalElapsed)
                AnimateSafePoints();

            if (ChannelDurationTimer.IntervalElapsed)
                EndChanneling();
        } else if (ChannelIntervalTimer.IntervalElapsed)
            StartChanneling();
    }
}