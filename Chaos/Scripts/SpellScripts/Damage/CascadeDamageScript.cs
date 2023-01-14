using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts.Damage;

public class CascadeDamageScript : ConfigurableSpellScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }

    protected CascadeAbilityComponent CascadeAbilityComponent { get; }
    protected CascadeAbilityComponent.CascadeAbilityComponentOptions CascadeAbilityComponentOptions { get; }
    protected DamageComponent DamageComponent { get; }
    protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

    protected IEffectFactory EffectFactory { get; }

    /// <inheritdoc />
    public CascadeDamageScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        CascadeAbilityComponent = new CascadeAbilityComponent();

        ApplyDamageScript = DefaultApplyDamageScript.Create();
        DamageComponent = new DamageComponent();

        return affectedPoints;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var direction = context.Source.Direction;

        ShowBodyAnimation(context);

        var allPossiblePoints = GetAffectedPoints(context)
                                .Cast<IPoint>()
                                .ToList();

        var elapsedMs = MinSoundDelayMs;

        _ = Task.Run(
            async () =>
            {
                for (var i = 1; i <= Range; i++)
                {
                    //get points for this stage
                    var pointsForStage = SelectPointsForStage(
                            allPossiblePoints,
                            context.SourcePoint,
                            direction,
                            i)
                        .ToList();

                    // ReSharper disable once RemoveRedundantBraces
                    await using (_ = await context.Map.Sync.WaitAsync())
                    {
                        try
                        {
                            ShowAnimation(context, pointsForStage);

                            var affectedEntitiesForStage = GetAffectedEntities<Creature>(context, pointsForStage);
                            ApplyDamage(context, affectedEntitiesForStage);

                            if (Sound.HasValue && elapsedMs >= MinSoundDelayMs)
                            {
                                PlaySound(context, pointsForStage);

                                elapsedMs = 0;
                            }
                        }
                        catch
                        {
                            //ignored
                        }
                    }

                    await Task.Delay(PropagationDelayMs);
                    elapsedMs += PropagationDelayMs;
                }
            });
    }

    private IEnumerable<IPoint> SelectPointsForStage(
        IEnumerable<IPoint> allPossiblePoints,
        Point sourcePoint,
        Direction aoeDirection,
        int range
    )
    {
        DamageComponentOptions = new DamageComponent.DamageComponentOptions
        {
            ApplyDamageScript = ApplyDamageScript,
            SourceScript = this,
            BaseDamage = BaseDamage,
            DamageMultiplier = DamageMultiplier,
            DamageStat = DamageStat
        };

        CascadeAbilityComponentOptions = new CascadeAbilityComponent.CascadeAbilityComponentOptions
        {
            Shape = Shape,
            Range = Range,
            Sound = Sound,
            Filter = Filter,
            Animation = Animation,
            BodyAnimation = BodyAnimation,
            AnimatePoints = AnimatePoints,
            IncludeSourcePoint = IncludeSourcePoint,
            DamageComponentOptions = DamageComponentOptions,
            PropagationIntervalMs = PropagationIntervalMs,
            SoundIntervalMs = SoundIntervalMs,
            StopAtWalls = StopAtWalls,
            EffectKey = EffectKey
        };
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context) =>
        CascadeAbilityComponent.Activate(context, CascadeAbilityComponentOptions, EffectFactory);

    #region ScriptVars
    protected AoeShape Shape { get; init; }
    protected int Range { get; init; }
    protected TargetFilter? Filter { get; init; }
    protected BodyAnimation? BodyAnimation { get; init; }
    protected Animation? Animation { get; init; }
    protected byte? Sound { get; init; }
    protected bool AnimatePoints { get; init; } = true;
    protected bool MustHaveTargets { get; init; }
    protected bool IncludeSourcePoint { get; init; } = true;
    protected int SoundIntervalMs { get; init; }
    protected int PropagationIntervalMs { get; init; }
    protected bool StopAtWalls { get; init; }
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageMultiplier { get; init; }
    protected string? EffectKey { get; init; }
    #endregion
}