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

namespace Chaos.Scripting.MonsterScripts.Boss.MythicBosses.GargoyleBoss.Gargoyle1;

public sealed class Gargoyle1BossThrowHazardousScript : MonsterScriptBase
{
    private Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 378
    };

    private IApplyDamageScript ApplyDamageScript { get; }
    private IIntervalTimer SpawnTiles { get; }

    /// <inheritdoc />
    public Gargoyle1BossThrowHazardousScript(Monster subject)
        : base(subject)
    {
        SpawnTiles = new RandomizedIntervalTimer(
            TimeSpan.FromSeconds(7),
            45,
            RandomizationType.Positive,
            false);

        ApplyDamageScript = ApplyAttackDamageScript.Create();
    }

    private Aisling? FindLowestAggro()
        => Subject.MapInstance
                  .GetEntitiesWithinRange<Aisling>(Subject, AggroRange)
                  .ThatAreObservedBy(Subject)
                  .ThatAreVisibleTo(Subject)
                  .FirstOrDefault(
                      obj => !obj.Equals(Subject)
                             && obj.IsAlive
                             && (obj.Id
                                 == Subject.AggroList.FirstOrDefault(a => a.Value == Subject.AggroList.Values.Min())
                                           .Key));

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

        var target = FindLowestAggro();

        if (target != null)
        {
            var options = new AoeShapeOptions
            {
                Source = new Point(target.X, target.Y),
                Range = 1
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
                var damage = (int)(aisling.StatSheet.EffectiveMaximumHp * 0.4);

                ApplyDamageScript.ApplyDamage(
                    Subject,
                    aisling,
                    this,
                    damage,
                    Element.Darkness);

                target.ShowHealth();
            }
        }
    }
}