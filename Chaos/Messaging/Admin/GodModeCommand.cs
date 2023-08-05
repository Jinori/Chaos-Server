using System.Runtime.InteropServices.ComTypes;
using Chaos.Collections.Common;
using Chaos.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command ("godmode")]

public class GodModeCommand : ICommand<Aisling>
{
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        source.StatSheet.SetHealthPct(100);
        source.ShowHealth();
        
        if (source.Trackers.Enums.TryGetValue(out GodMode godMode) && (godMode != GodMode.Yes))
            source.Trackers.Enums.Set(GodMode.Yes);
        else
            source.Trackers.Enums.Set(GodMode.No);

        return default;
    }
}