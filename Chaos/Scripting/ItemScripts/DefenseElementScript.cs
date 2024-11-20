using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class DefenseElementScript : ConfigurableItemScriptBase
{
    protected Element Element { get; init; }

    public DefenseElementScript(Item subject)
        : base(subject) { }

    public override void OnEquipped(Aisling aisling)
    {
        aisling.StatSheet.SetDefenseElement(Element);
        aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }

    public override void OnUnEquipped(Aisling aisling)
    {
        aisling.StatSheet.SetDefenseElement(Element.None);
        aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }
}