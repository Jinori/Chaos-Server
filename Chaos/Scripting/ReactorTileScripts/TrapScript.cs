#region
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.ReactorTileScripts;

public class TrapScript : ConfigurableReactorTileScriptBase,
                          GetTargetsAbilityComponent<Creature>.IGetTargetsComponentOptions,
                          SoundAbilityComponent.ISoundComponentOptions,
                          AnimationAbilityComponent.IAnimationComponentOptions,
                          TrapDamageAbilityComponent.ITrapDamageComponentOptions,
                          ManaDrainAbilityComponent.IManaDrainComponentOptions,
                          ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{
    protected IIntervalTimer AnimationTimer { get; set; }
    protected Creature Owner { get; set; }
    protected IIntervalTimer? Timer { get; set; }
    protected int TriggerCount { get; set; }

    protected Animation DetectTrapAnimation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 96
    };

    /// <inheritdoc />
    public TrapScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        if (Subject.Owner == null)
            throw new Exception(
                $"""{nameof(TrapScript)} script initialized for {Subject} that has no owner. If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey". If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{nameof(IReactorTileFactory.Create)}() call.""");

        Owner = subject.Owner!;
        EffectFactory = effectFactory;
        TriggerCount = 0;

        if (DurationSecs.HasValue)
            Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs.Value), false);

        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        if (!Filter.IsValidTarget(Owner, source))
            return;

        if (source.IsGodModeEnabled() || source.Effects.Contains("invulnerability"))
            return;

        var executed = new ComponentExecutor(Owner, source).WithOptions(this)
                                                           .ExecuteAndCheck<GetTargetsAbilityComponent<Creature>>()
                                                           ?.Execute<SoundAbilityComponent>()
                                                           .Execute<AnimationAbilityComponent>()
                                                           .Execute<TrapDamageAbilityComponent>()
                                                           .Execute<ManaDrainAbilityComponent>()
                                                           .Execute<ApplyEffectAbilityComponent>()
                       != null;

        if (Owner is Aisling && source is Aisling aisling)
            aisling.SendOrangeBarMessage($"You stumble upon a trap, set by {Owner.Name}!");

        if (executed && MaxTriggers.HasValue)
        {
            TriggerCount++;

            if (TriggerCount >= MaxTriggers)
                Map.RemoveEntity(Subject);
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        AnimationTimer.Update(delta);

        if (Subject.Owner is not null
            && Subject.Owner.IsDetectingTraps()
            && AnimationTimer.IntervalElapsed
            && Subject.MapInstance.Equals(Subject.Owner.MapInstance))
            Subject.Owner.MapInstance.ShowAnimationToFriendly(
                DetectTrapAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y), Subject.Owner.Id),
                Subject.Owner);

        if (Timer != null)
        {
            Timer.Update(delta);

            if (Timer.IntervalElapsed)
                Map.RemoveEntity(Subject);
        }
    }

    #region ScriptVars
    /// <inheritdoc />
    public TimeSpan? EffectDurationOverride { get; init; }

    public IEffectFactory EffectFactory { get; init; }
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public int Range { get; init; }

    /// <inheritdoc />
    public int? ExclusionRange { get; init; }

    public TargetFilter Filter { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; }
    public bool MustHaveTargets { get; init; } = true;
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public bool? MoreDmgLowTargetHp { get; init; }

    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public Element? Element { get; init; }
    public int? DurationSecs { get; init; }
    public int? MaxTriggers { get; init; }
    public string? EffectKey { get; init; }
    public int? EffectApplyChance { get; init; }
    public int? ManaDrain { get; init; }
    public decimal PctManaDrain { get; init; }
    #endregion
}