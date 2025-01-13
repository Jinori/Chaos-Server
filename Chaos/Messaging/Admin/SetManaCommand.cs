using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("setmp", helpText: "<mp>")]
public class SetManaCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext(out int mp))
            return default;

        source.UserStatSheet.SetMaxMp(mp);
        source.UserStatSheet.SetManaPct(100);

        source.Client.SendAttributes(StatUpdateType.Full);

        return default;
    }
}