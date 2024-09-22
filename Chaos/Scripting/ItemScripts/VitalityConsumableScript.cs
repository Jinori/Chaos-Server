using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using NLog.Targets;

namespace Chaos.Scripting.ItemScripts;

public class VitalityConsumableScript : ConfigurableItemScriptBase,
                                        GenericAbilityComponent<Aisling>.IAbilityComponentOptions,
                                        DamageAbilityComponent.IDamageComponentOptions,
                                        HealAbilityComponent.IHealComponentOptions,
                                        ManaDrainAbilityComponent.IManaDrainComponentOptions,
                                        ManaReplenishAbilityComponent.IManaReplenishComponentOptions,
                                        ApplyEffectAbilityComponent.IApplyEffectComponentOptions

{
    public bool AllAggro { get; set; }

    /// <inheritdoc />
    public VitalityConsumableScript(Item subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        ApplyHealScript.HealFormula = HealFormulae.Default;
        SourceScript = this;
        ItemName = Subject.DisplayName;
    }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        if (!source.UserStatSheet.Master && Subject.Template.RequiresMaster)
        {
            source.SendOrangeBarMessage($"You must be a Master to consume this.");
            return;
        }
        
        if (source.UserStatSheet.Level < Subject.Level && !source.IsGodModeEnabled())
        {
            source.SendOrangeBarMessage($"You must be level {Subject.Level} to consume this.");
            return;
        }

        if (EffectKey != null && source.Effects.Contains(EffectKey))
        {
            source.SendOrangeBarMessage("You already have that effect.");
            return;
        }
        
        if (source.Trackers.TimedEvents.HasActiveEvent("potiontimer", out var cdtimer))
        {
            source.SendOrangeBarMessage($"You must wait {cdtimer.Remaining.Seconds} seconds before consuming something else.");
            return;
        }
        
        source.Inventory.RemoveQuantity(ItemName, 1);
        
        source.Trackers.TimedEvents.AddEvent("potiontimer", TimeSpan.FromSeconds(6),true);
        
        if (Message)
            source.SendOrangeBarMessage("You consumed a " + ItemName + ".");
        
        new ComponentExecutor(source, source).WithOptions(this)
            .ExecuteAndCheck<GenericAbilityComponent<Aisling>>()
            ?.Execute<DamageAbilityComponent>()
            .Execute<HealAbilityComponent>()
            .Execute<ManaDrainAbilityComponent>()
            .Execute<ManaReplenishAbilityComponent>()
            .Execute<ApplyEffectAbilityComponent>();
        
    }

    #region ScriptVars
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public bool SingleTarget { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    public bool StopOnWalls { get; init; }
    public bool StopOnFirstHit { get; init; }

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }

    /// <inheritdoc />
    public bool? ScaleBodyAnimationSpeedByAttackSpeed { get; init; }

    /// <inheritdoc />
    public ushort? AnimationSpeed { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public bool AnimatePoints { get; init; }

    /// <inheritdoc />
    public int? ManaCost { get; init; }

    /// <inheritdoc />
    public decimal PctManaCost { get; init; }

    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }

    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }

    /// <inheritdoc />
    public int? BaseDamage { get; init; }
    /// <inheritdoc />
    public bool? MoreDmgLowTargetHp { get; init; }
    /// <inheritdoc />
    public Stat? DamageStat { get; init; }

    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }

    /// <inheritdoc />
    public IApplyHealScript ApplyHealScript { get; init; }

    /// <inheritdoc />
    public int? BaseHeal { get; init; }

    /// <inheritdoc />
    public Stat? HealStat { get; init; }

    /// <inheritdoc />
    public decimal? HealStatMultiplier { get; init; }

    /// <inheritdoc />
    public decimal? PctHpHeal { get; init; }

    public IScript SourceScript { get; init; }
    public bool? SurroundingTargets { get; init; }
    public decimal? DamageMultiplierPerTarget { get; init; }

    /// <inheritdoc />
    public int? ManaDrain { get; init; }

    /// <inheritdoc />
    public decimal PctManaDrain { get; init; }

    /// <inheritdoc />
    public int? ManaReplenish { get; init; }

    /// <inheritdoc />
    public decimal PctManaReplenish { get; init; }

    /// <inheritdoc />
    public bool ReplenishGroup { get; init; }
    /// <inheritdoc />
    public string ItemName { get; init; }

    public bool Message { get; init; } = true;
    public int Level { get; init; }

    /// <inheritdoc />
    public TimeSpan? EffectDurationOverride { get; init; }
    /// <inheritdoc />
    public IEffectFactory EffectFactory { get; init; }
    /// <inheritdoc />
    public string? EffectKey { get; init; }

    public int? EffectApplyChance { get; init; }

    #endregion

    public int SplashChance { get; init; }
    public int SplashDistance { get; init; }
    public TargetFilter SplashFilter { get; init; }
}