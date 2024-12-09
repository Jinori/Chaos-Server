using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class HealZoneScript : ConfigurableReactorTileScriptBase,
                              GetTargetsAbilityComponent<Creature>.IGetTargetsComponentOptions,
                              SoundAbilityComponent.ISoundComponentOptions,
                              AnimationAbilityComponent.IAnimationComponentOptions,
                              HealAbilityComponent.IHealComponentOptions,
                              ManaDrainAbilityComponent.IManaDrainComponentOptions,
                              ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{
    protected IIntervalTimer AnimationTimer { get; set; }

    protected IIntervalTimer HealTimer { get; set; }
    protected Creature Owner { get; set; }
    protected IIntervalTimer? Timer { get; set; }
    protected int TriggerCount { get; set; }

    /// <inheritdoc />
    public HealZoneScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        ApplyHealScript = FunctionalScripts.ApplyHealing.ApplyHealScript.Create();

        if (Subject.Owner == null)
            throw new Exception(
                $"""{nameof(TrapScript)} script initialized for {Subject} that has no owner. If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey". If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{nameof(IReactorTileFactory.Create)}() call.""");

        Owner = subject.Owner!;
        EffectFactory = effectFactory;
        TriggerCount = 0;

        if (DurationSecs.HasValue)
            Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs.Value), false);

        AnimationTimer = new IntervalTimer(TimeSpan.FromMilliseconds(1500), false);
        HealTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        SourceScript = this;
    }

    private void ApplyTrapEffects(Creature target)
    {
        if (!Filter.IsValidTarget(Owner, target))
            return;

        if (target.IsGodModeEnabled())
            return;

        var executed = new ComponentExecutor(Owner, target).WithOptions(this)
                                                           .ExecuteAndCheck<GetTargetsAbilityComponent<Creature>>()
                                                           ?.Execute<SoundAbilityComponent>()
                                                           .Execute<AnimationAbilityComponent>()
                                                           .Execute<HealAbilityComponent>()
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
    public override void OnWalkedOn(Creature source) => ApplyTrapEffects(source);

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        HealTimer.Update(delta);

        AnimationTimer.Update(delta);

        if (Subject.Owner is not null && AnimationTimer.IntervalElapsed && Subject.MapInstance.Equals(Subject.Owner.MapInstance))
            if (Animation != null)
                Subject.Owner.MapInstance.ShowAnimation(Animation.GetPointAnimation(Subject));

        if (HealTimer.IntervalElapsed)
            foreach (var creature in Subject.MapInstance.GetEntities<Creature>())
            {
                var creaturepoint = (creature.X, creature.Y);
                var trappoint = (Subject.X, Subject.Y);

                if (creaturepoint == trappoint)
                    ApplyTrapEffects(creature);
            }

        if (Timer != null)
        {
            Timer.Update(delta);

            if (Timer.IntervalElapsed)
                Map.RemoveEntity(Subject);
        }
    }

    #region ScriptVars
    public int? ExclusionRange { get; init; }
    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    public int? DurationSecs { get; init; }
    public int? MaxTriggers { get; init; }
    public TargetFilter Filter { get; init; }
    public bool MustHaveTargets { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public IApplyHealScript ApplyHealScript { get; init; }
    public int? BaseHeal { get; init; }
    public Stat? HealStat { get; init; }
    public decimal? HealStatMultiplier { get; init; }
    public bool? MoreHealLowTargetHp { get; init; }
    public decimal? PctHpHeal { get; init; }
    public IScript SourceScript { get; init; }
    public int? ManaDrain { get; init; }
    public decimal PctManaDrain { get; init; }
    public TimeSpan? EffectDurationOverride { get; init; }
    public IEffectFactory EffectFactory { get; init; }
    public string? EffectKey { get; init; }
    public int? EffectApplyChance { get; init; }
    #endregion
}