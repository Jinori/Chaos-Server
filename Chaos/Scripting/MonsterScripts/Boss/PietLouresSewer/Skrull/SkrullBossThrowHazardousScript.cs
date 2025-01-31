using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts.Boss.PietLouresSewer.Skrull;

public sealed class SkrullBossThrowHazardousScript : MonsterScriptBase
{
    private Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 235
    };

    private IApplyDamageScript ApplyDamageScript { get; }
    private IIntervalTimer SpawnTiles { get; }

    /// <inheritdoc />
    public SkrullBossThrowHazardousScript(Monster subject)
        : base(subject)
    {
        SpawnTiles = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(7),
            45,
            RandomizationType.Positive,
            false);

        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        SpawnTiles.Update(delta);

        if (!Map.GetEntities<Aisling>()
                .Any())
            return;

        if (!SpawnTiles.IntervalElapsed)
            return;

        if (Target != null)
        {
            var options = new AoeShapeOptions
            {
                Source = new Point(Target.X, Target.Y),
                Range = 3
            };

            var points = AoeShape.AllAround.ResolvePoints(options);

            var enumerable = points as Point[] ?? points.ToArray();

            foreach (var tile in enumerable)
                Subject.MapInstance.ShowAnimation(Animation.GetPointAnimation(tile));

            var targets = Subject.MapInstance
                                 .GetEntitiesAtPoints<Aisling>(enumerable.Cast<IPoint>())
                                 .WithFilter(Subject, TargetFilter.HostileOnly)
                                 .ToList();

            foreach (var aisling in targets)
            {
                var damage = (int)(aisling.StatSheet.EffectiveMaximumHp * 0.2);

                ApplyDamageScript.ApplyDamage(
                    Subject,
                    aisling,
                    this,
                    damage,
                    Element.Fire);

                aisling.ShowHealth();
            }
        }
    }
}