using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class PorteTrapScript : ConfigurableReactorTileScriptBase,
                          GetTargetsComponent<Creature>.IGetTargetsComponentOptions,
                          SoundComponent.ISoundComponentOptions,
                          AnimationComponent.IAnimationComponentOptions,
                          DamageComponent.IDamageComponentOptions,
                          ManaDrainComponent.IManaDrainComponentOptions,
                          ApplyEffectComponent.IApplyEffectComponentOptions
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
    public PorteTrapScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        if (Subject.Owner == null)
            throw new Exception(
                $"""
{nameof(TrapScript)} script initialized for {Subject} that has no owner. 
If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey". 
If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{
    nameof(IReactorTileFactory.Create)}() call.
""");

        Owner = subject.Owner!;
        EffectFactory = effectFactory;
        TriggerCount = 0;

        if (DurationSecs.HasValue)
            Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs.Value), false);
        
        AnimationTimer = new IntervalTimer(TimeSpan.FromSeconds(1), false);
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        //if the person who stepped on it isnt a valid target, do nothing
        if (!Filter.IsValidTarget(Owner, source))
            return;

        var aisling = source as Aisling;

        if ((aisling?.MapInstance.InstanceId == "pf_path") && aisling.Inventory.HasCount("Giant Ant Wing", 1))
        {
            aisling.Inventory.RemoveQuantity("Giant Ant Wing", 1);
            aisling.SendOrangeBarMessage($"You lay down a Giant Ant Wing to avoid the trap, {aisling.Inventory.CountOf("Giant Ant Wing")} left.");

            return;
        }
        if ((aisling?.MapInstance.InstanceId == "karlopostrap") && aisling.Inventory.HasCount("Giant Ant Wing", 1))
        {
            aisling.Inventory.RemoveQuantity("Giant Ant Wing", 1);
            aisling.SendOrangeBarMessage($"You lay down a Giant Ant Wing to avoid the trap, {aisling.Inventory.CountOf("Giant Ant Wing")} left.");

            return;
        }
        
        var executed = new ComponentExecutor(Owner, source)
                       .WithOptions(this)
                       .ExecuteAndCheck<GetTargetsComponent<Creature>>()
                       ?
                       .Execute<SoundComponent>()
                       .Execute<AnimationComponent>()
                       .Execute<DamageComponent>()
                       .Execute<ManaDrainComponent>()
                       .Execute<ApplyEffectComponent>()
                       != null;

        if (executed && MaxTriggers.HasValue)
        {
            TriggerCount++;

            if (TriggerCount >= MaxTriggers)
                Map.RemoveObject(Subject);
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        AnimationTimer.Update(delta);

        if (Subject.Owner is not null
            && Subject.Owner.Status.HasFlag(Status.DetectTraps)
            && AnimationTimer.IntervalElapsed
            && Subject.MapInstance.Equals(Subject.Owner.MapInstance))
            Subject.Owner.MapInstance.ShowAnimation(
                DetectTrapAnimation.GetPointAnimation(new Point(Subject.X, Subject.Y), Subject.Owner.Id));

        if (Timer !=null)
        {
            Timer.Update(delta);
            if (Timer.IntervalElapsed)
                Map.RemoveObject(Subject);
        }
    }

    #region ScriptVars
    public IEffectFactory EffectFactory { get; init; }
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public AoeShape Shape { get; init; }
    public BodyAnimation BodyAnimation { get; init; }
    public int Range { get; init; }
    public TargetFilter Filter { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; }
    public bool MustHaveTargets { get; init; } = true;
    public bool ExcludeSourcePoint { get; init; }
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public IScript SourceScript { get; init; }
    public Element? Element { get; init; }
    public int? DurationSecs { get; init; }
    public int? MaxTriggers { get; init; }
    public string? EffectKey { get; init; }
    public int? ManaDrain { get; init; }
    public decimal PctManaDrain { get; init; }
    #endregion
}