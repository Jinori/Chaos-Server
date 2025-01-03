using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class SetElementScript : ConfigurableItemScriptBase
{
    /// <inheritdoc />
    public SetElementScript(Item subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnEquipped(Aisling aisling)
    {
        if (OffenseElement.HasValue)
            aisling.StatSheet.SetOffenseElement(OffenseElement.Value);

        if (DefenseElement.HasValue)
            aisling.StatSheet.SetDefenseElement(DefenseElement.Value);

        if (OffenseElement.HasValue || DefenseElement.HasValue)
            aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }

    /// <inheritdoc />
    public override void OnUnEquipped(Aisling aisling)
    {
        if (OffenseElement.HasValue)
            aisling.StatSheet.SetOffenseElement(Element.None);

        if (DefenseElement.HasValue)
            aisling.StatSheet.SetDefenseElement(Element.None);

        if (OffenseElement.HasValue || DefenseElement.HasValue)
            aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }

    #region ScriptVars
    public Element? OffenseElement { get; init; }
    public Element? DefenseElement { get; init; }
    #endregion
}