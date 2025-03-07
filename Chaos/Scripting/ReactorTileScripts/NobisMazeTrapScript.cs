using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class NobisMazeTrapScript : ConfigurableReactorTileScriptBase,
                                   GetTargetsAbilityComponent<Creature>.IGetTargetsComponentOptions,
                                   SoundAbilityComponent.ISoundComponentOptions,
                                   AnimationAbilityComponent.IAnimationComponentOptions,
                                   DamageAbilityComponent.IDamageComponentOptions,
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
    public NobisMazeTrapScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        if (Subject.Owner == null)
            throw new Exception(
                $"""
                 {nameof(TrapScript)} script initialized for {Subject} that has no owner.
                 If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey".
                 If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{nameof(IReactorTileFactory.Create)}() call.
                 """);

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
        //if the person who stepped on it isnt a valid target, do nothing
        if (!Filter.IsValidTarget(Owner, source))
            return;

        if (source.IsGodModeEnabled())
            return;

        var executed = new ComponentExecutor(Owner, source).WithOptions(this)
                                                           .ExecuteAndCheck<GetTargetsAbilityComponent<Creature>>()
                                                           ?.Execute<SoundAbilityComponent>()
                                                           .Execute<AnimationAbilityComponent>()
                                                           .Execute<DamageAbilityComponent>()
                                                           .Execute<ManaDrainAbilityComponent>()
                                                           .Execute<ApplyEffectAbilityComponent>()
                       != null;

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

        if (AnimationTimer.IntervalElapsed)
        {
            var detectingAislings = Subject.MapInstance
                                           .GetEntitiesWithinRange<Aisling>(Subject, 5)
                                           .Where(aisling => aisling.IsDetectingTraps());

            foreach (var aisling in detectingAislings)
                aisling.MapInstance.ShowAnimation(DetectTrapAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));

            var nearbyAdmins = Map.GetEntitiesWithinRange<Aisling>(Subject)
                                  .Where(aisling => aisling.Trackers.Enums.HasValue(GodMode.Yes));

            foreach (var nearbyAdmin in nearbyAdmins)
                nearbyAdmin.Client.SendAnimation(DetectTrapAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y)));
        }

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

    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    public BodyAnimation BodyAnimation { get; init; }
    public int Range { get; init; }
    public TargetFilter Filter { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; }
    public bool MustHaveTargets { get; init; } = true;
    public int? ExclusionRange { get; init; }
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public bool? MoreDmgLowTargetHp { get; init; }

    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public decimal? PctOfHealthMultiplier { get; init; }
    public decimal? PctOfHealth { get; init; }
    public bool? SurroundingTargets { get; init; }
    public decimal? DamageMultiplierPerTarget { get; init; }
    public decimal? PctOfMana { get; init; }
    public decimal? PctOfManaMultiplier { get; init; }
    public Element? Element { get; init; }
    public int? DurationSecs { get; init; }
    public int? MaxTriggers { get; init; }
    public string? EffectKey { get; init; }
    public int? EffectApplyChance { get; init; }
    public int? ManaDrain { get; init; }
    public decimal PctManaDrain { get; init; }
    #endregion
}