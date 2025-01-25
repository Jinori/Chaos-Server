using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
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

namespace Chaos.Scripting.ReactorTileScripts;

public class MeteorFireScript : ConfigurableReactorTileScriptBase,
                                GetTargetsAbilityComponent<Creature>.IGetTargetsComponentOptions,
                                SoundAbilityComponent.ISoundComponentOptions,
                                AnimationAbilityComponent.IAnimationComponentOptions,
                                TrapDamageAbilityComponent.ITrapDamageComponentOptions,
                                ManaDrainAbilityComponent.IManaDrainComponentOptions,
                                ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{ 
    protected IIntervalTimer DamageTimer { get; set; }
    
    public ComponentExecutor Executor { get; init; }
    protected Creature Owner { get; set; }
    protected IIntervalTimer? Timer { get; set; }
    protected int TriggerCount { get; set; }

    /// <inheritdoc />
    public MeteorFireScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        
        var context = new ActivationContext(Subject.Owner!, Subject);
        var vars = new ComponentVars();
        
        if (Subject.Owner == null)
            throw new Exception(
                $"""{nameof(TrapScript)} script initialized for {Subject} that has no owner. If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey". If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{nameof(IReactorTileFactory.Create)}() call.""");

        Owner = subject.Owner!;
        EffectFactory = effectFactory;
        TriggerCount = 0;

        if (DurationSecs.HasValue)
            Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs.Value), false);
        
        DamageTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        
        Executor = new ComponentExecutor(context, vars).WithOptions(this);
        
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        DamageTimer.Update(delta);
        
        if (DamageTimer.IntervalElapsed)
        {
            if (Owner is Aisling aisling)
            {
                if (aisling.IsGodModeEnabled())
                    return;

                if (aisling.IsHostileTo(Owner))
                    aisling.SendOrangeBarMessage("You're burned by fire from Meteor!");
            }


            Executor.ExecuteAndCheck<GetTargetsAbilityComponent<Creature>>()
                    ?.Execute<SoundAbilityComponent>()
                    .Execute<AnimationAbilityComponent>()
                    .Execute<TrapDamageAbilityComponent>()
                    .Execute<ManaDrainAbilityComponent>()
                    .Execute<ApplyEffectAbilityComponent>();
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
    public TargetFilter Filter { get; init; }
    public bool MustHaveTargets { get; init; }
    public int Range { get; init; }
    public AoeShape Shape { get; init; }
    public bool SingleTarget { get; init; }
    public bool StopOnFirstHit { get; init; }
    public bool StopOnWalls { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; }
    public Animation? Animation { get; init; }
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public Element? Element { get; init; }
    public bool? MoreDmgLowTargetHp { get; init; }
    public decimal? PctHpDamage { get; init; }
    public int? ManaDrain { get; init; }
    public decimal PctManaDrain { get; init; }
    public int? EffectApplyChance { get; init; }
    public TimeSpan? EffectDurationOverride { get; init; }
    public IEffectFactory EffectFactory { get; init; }
    public string? EffectKey { get; init; }
    public int? DurationSecs { get; init; }
  
    #endregion
}