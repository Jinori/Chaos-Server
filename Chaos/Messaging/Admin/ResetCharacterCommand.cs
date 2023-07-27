using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("resetchar")]
public class ResetCharacterCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        source.UserStatSheet.SetLevel(1);
        source.UserStatSheet.SubtractTotalExp(source.UserStatSheet.TotalExp);

        if (source.UserStatSheet.ToNextLevel >= 1)
            source.UserStatSheet.SubtractTnl(source.UserStatSheet.ToNextLevel);

        source.UserStatSheet.AddTnl(599);
        source.UserStatSheet.Str = 1;
        source.UserStatSheet.Int = 1;
        source.UserStatSheet.Wis = 1;
        source.UserStatSheet.Con = 1;
        source.UserStatSheet.Dex = 1;
        source.UserStatSheet.SetMaxWeight(51);
        source.UserStatSheet.UnspentPoints = 0;

        var statBuyCost = new Attributes
        {
            MaximumHp = source.UserStatSheet.MaximumHp - 100,
            MaximumMp = source.UserStatSheet.MaximumMp - 100
        };

        source.UserStatSheet.Subtract(statBuyCost);
        source.Client.SendAttributes(StatUpdateType.Full);
        source.Refresh(true);

        return default;
    }
}