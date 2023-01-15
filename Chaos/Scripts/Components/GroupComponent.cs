using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.Components;

public class GroupComponent
{
    public virtual void ApplyHealing(ActivationContext context, IReadOnlyCollection<Aisling> targetEntities, GroupComponentOptions options)
    {
        var healing = CalculateHealing(
            context,
            options.BaseHealing,
            options.HealStat,
            options.HealStatMultiplier);

        if (healing == 0)
            return;

        foreach (var target in targetEntities)
        {
            target.ApplyHealing(context.Source, healing);
        }
    }

    protected virtual int CalculateHealing(
        ActivationContext context,
        int? BaseHealing = null,
        Stat? HealStat = null,
        decimal? HealStatMultiplier = null
    )
    {
        var heals = BaseHealing ?? 0;

        if (HealStat.HasValue)
        {
            var multiplier = HealStatMultiplier ?? 1; 
            heals += Convert.ToInt32(context.Source.StatSheet.GetEffectiveStat(HealStat.Value) * multiplier);
        }

        return heals;
    }
    
    public virtual (List<IPoint> targetPoints, List<Aisling>? targetEntities) Activate<T>(ActivationContext context,
        GroupComponentOptions options) where T: MapEntity
    {
        var targetPoints = options.Shape.ResolvePoints(
                                      context.TargetPoint,
                                      options.Range,
                                      context.Target.Direction,
                                      null,
                                      options.IncludeSourcePoint)
                                  .ToListCast<IPoint>();

        var targetEntities = context.SourceAisling?.Group?.Where(x => x.WithinRange(context.SourcePoint)).ToList();

        if (options.MustHaveTargets && !targetEntities.Any())
            return (targetPoints, targetEntities);

        if (options.BodyAnimation.HasValue)
            context.Source.AnimateBody(options.BodyAnimation.Value);

        if (options.Animation != null)
            if (options.AnimatePoints)
                foreach (var point in targetPoints)
                    context.Map.ShowAnimation(options.Animation.GetPointAnimation(point, context.Source.Id));
            else
                foreach (var target in targetEntities)
                    target.Animate(options.Animation, context.Source.Id);

        if (options.Sound.HasValue)
            context.Map.PlaySound(options.Sound.Value, targetPoints);

        return (targetPoints, targetEntities);
    }

    // ReSharper disable once ClassCanBeSealed.Global
    public class GroupComponentOptions
    {
        public int? BaseHealing { get; init; }
        public Stat? HealStat { get; init; }
        public decimal? HealStatMultiplier { get; init; }
        public bool AnimatePoints { get; init; }
        public Animation? Animation { get; init; }
        public BodyAnimation? BodyAnimation { get; init; }
        public bool IncludeSourcePoint { get; init; }
        public bool MustHaveTargets { get; init; }
        public required int Range { get; init; }
        public required AoeShape Shape { get; init; }
        public byte? Sound { get; init; }
        public required IScript SourceScript { get; init; }
    }
}