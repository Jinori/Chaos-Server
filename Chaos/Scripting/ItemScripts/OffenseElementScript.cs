﻿using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class OffenseElementScript : ConfigurableItemScriptBase
{
    protected Element Element { get; init; }

    public OffenseElementScript(Item subject)
        : base(subject) { }

    public override void OnEquipped(Aisling aisling)
    {
        aisling.StatSheet.SetOffenseElement(Element);
        aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }

    public override void OnUnEquipped(Aisling aisling)
    {
        aisling.StatSheet.SetOffenseElement(Element.None);
        aisling.Client.SendAttributes(StatUpdateType.Secondary);
    }
}