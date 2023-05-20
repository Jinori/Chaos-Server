using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts.Warrior;

public class ExecuteScript : ConfigurableSkillScriptBase, AbilityComponent<Creature>.IAbilityComponentOptions,
                             ExecuteComponent.IExecuteComponentOptions
{
    /// <inheritdoc />
    public ExecuteScript(Skill subject)
        : base(subject)
    {
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context) =>
        new ComponentExecutor(context)
            .WithOptions(this)
            .ExecuteAndCheck<AbilityComponent<Creature>>()
            ?
            .Execute<ExecuteComponent>();
    

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
    /// <inheritdoc />
    public IApplyHealScript ApplyHealScript { get; init; }
    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }
    /// <inheritdoc />
    public IScript SourceScript { get; init; }
    /// <inheritdoc />
    public int? KillTargetAtHealthPct { get; init; }
    /// <inheritdoc />
    public int? CooldownReduction { get; init; }
    /// <inheritdoc />
    public decimal HealAmountIfExecuted { get; init; }
    /// <inheritdoc />
    public decimal DmgHealthPct { get; init; }
}