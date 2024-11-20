using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command ("godmode")]

public class GodModeCommand : ICommand<Aisling>
{
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {

        if (!source.IsGodModeEnabled())
        {
            source.StatSheet.SetHealthPct(100);
            source.Client.SendAttributes(StatUpdateType.Vitality);
            source.SendOrangeBarMessage("Godmode enabled.");
            source.Trackers.Enums.Set(GodMode.Yes);

            return default;
        }
        
        source.SendOrangeBarMessage("Godmode disabled.");
        source.Trackers.Enums.Set(GodMode.No);

        return default;
    }
}