using Chaos.Common.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class VitalityConsumableScript : ConfigurableItemScriptBase
{
    /// <inheritdoc />
    public VitalityConsumableScript(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnUse(Aisling source)
    {
        if (HealthAmount.HasValue)
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