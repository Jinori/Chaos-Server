using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class HealScript : BasicSpellScriptBase, HealComponent.IHealComponentOptions
{
    public IApplyHealScript ApplyHealScript { get; init; }
    public IScript SourceScript { get; init; }
    protected HealComponent HealComponent { get; }

    /// <inheritdoc />
    public HealScript(Spell subject)
        : base(subject)
    {
        ApplyHealScript = FunctionalScripts.ApplyHealing.HealScript.Create();
        HealComponent = new HealComponent();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        HealComponent.ApplyHeal(context, targets.TargetEntities, this);
        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    public int? BaseHeal { get; init; }
    public Stat? HealStat { get; init; }
    public decimal? HealStatMultiplier { get; init; }
    public decimal? PctHpHeal { get; init; }
    #endregion
}