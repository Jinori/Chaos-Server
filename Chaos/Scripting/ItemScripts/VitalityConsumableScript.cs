using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.FunctionalScripts.ApplyHealing;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class VitalityConsumableScript : ConfigurableItemScriptBase
{
    private readonly IApplyDamageScript ApplyDamageScript;
    private readonly IApplyHealScript ApplyHealScript;
    protected ManaCostComponent ManaCostComponent { get; }

    /// <inheritdoc />
    public VitalityConsumableScript(Item subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyNonAttackDamageScript.Create();
        ApplyHealScript = ApplyNonAlertingHealScript.Create();
        ManaCostComponent = new ManaCostComponent();
    }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        var hpAmt = HealthAmount ?? 0;

        if (HealthPercent.HasValue)
            hpAmt += MathEx.GetPercentOf<int>((int)source.StatSheet.EffectiveMaximumHp, HealthPercent.Value);

        switch (hpAmt)
        {
            
            case < 0:
                ApplyDamageScript.ApplyDamage(
                    source,
                    source,
                    this,
                    hpAmt);
                

                break;
            case > 0:
                ApplyHealScript.ApplyHeal(
                    source,
                    source,
                    this,
                    hpAmt);

                break;
        }

        if (HealthAmount.HasValue && (source.StatSheet.CurrentHp + HealthAmount.Value) > 0)
            source.UserStatSheet.AddHp(HealthAmount.Value);

        if (HealthPercent.HasValue)
            source.UserStatSheet.AddHealthPct(HealthPercent.Value);

        if (ManaAmount.HasValue)
            source.UserStatSheet.AddMp(ManaAmount.Value);

        if (ManaPercent.HasValue)
            source.UserStatSheet.AddManaPct(ManaPercent.Value);

        source.Client.SendAttributes(StatUpdateType.Vitality);
        source.Inventory.RemoveQuantity(Subject.Slot, 1);
    }

    #region ScriptVars

    private int? HealthAmount { get; init; }
    private int? HealthPercent { get; init; }
    private int? ManaAmount { get; init; }
    private int? ManaPercent { get; init; }
    
    #endregion
}