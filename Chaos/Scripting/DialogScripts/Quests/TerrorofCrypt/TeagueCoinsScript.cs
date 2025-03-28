﻿using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Quests.TerrorofCrypt;

public class TeagueCoinsScript : DialogScriptBase
{
    public TeagueCoinsScript(Dialog subject)
        : base(subject) { }

    public override void OnDisplaying(Aisling source)
    {
        if (source.Gold >= 1000)
        {
            source.TryTakeGold(1000);
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "The old man's eyes light up with glee.");
        }
        else
        {
            source.Client.SendServerMessage(ServerMessageType.OrangeBar1, "You reach into your coin purse but it's not that full...");
            Subject.Reply(source, "You almost had me excited there.. don't promise what you can't make true.");
        }
    }
}