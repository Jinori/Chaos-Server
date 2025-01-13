using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("sethp", helpText: "<hp>")]
public class SetHealthCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext(out int hp))
            return default;

        source.UserStatSheet.SetMaxHp(hp);
        source.UserStatSheet.SetHealthPct(100);

        source.Client.SendAttributes(StatUpdateType.Full);

        return default;
    }
}