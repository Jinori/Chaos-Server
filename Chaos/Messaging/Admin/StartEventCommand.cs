using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command ("startevent")]

public class StartEventCommand : ICommand<Aisling>
{
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {

        if (!source.Trackers.Enums.HasValue(StartEvent.StartEvent))
        {
            source.SendOrangeBarMessage("Start Event set to on.");
            source.Trackers.Enums.Set(StartEvent.StartEvent);

            return default;
        }
        
        source.SendOrangeBarMessage("Start Event set to off.");
        source.Trackers.Enums.Set(StartEvent.Off);

        return default;
    }
}