#region
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts.GuildBank;

public class GuildWithdrawGoldScript : DialogScriptBase
{
    private readonly ILogger<GuildWithdrawGoldScript> Logger;

    /// <inheritdoc />
    public GuildWithdrawGoldScript(Dialog subject, ILogger<GuildWithdrawGoldScript> logger)
        : base(subject)
        => Logger = logger;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source) => Subject.InjectTextParameters(source.Guild?.Bank.Gold!);

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!TryFetchArgs<int>(out var amount) || (amount <= 0))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if(source.Guild == null)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var guildRank = source.Guild.RankOf(source.Name);
        
        if(!guildRank.IsOfficerRank)
        {
            Subject.Reply(source, "You are not authorized to withdraw gold from the guild bank.", "top");

            return;
        }
        
        var withdrawResult = ComplexActionHelper.WithdrawGuildGold(source, amount);

        switch (withdrawResult)
        {
            case ComplexActionHelper.WithdrawGoldResult.Success:
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold, Topics.Actions.Withdraw)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(source.Guild)
                      .LogInformation(
                          "Aisling {@AislingName} withdrew {Amount} gold from the {GuildName} bank",
                          source,
                          amount,
                          source.Guild.Name);

                break;
            case ComplexActionHelper.WithdrawGoldResult.TooMuchGold:
                Subject.Reply(source, "You are carrying too much gold.", "generic_withdrawguildgold_initial");

                break;
            case ComplexActionHelper.WithdrawGoldResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that much.", "generic_withdrawguildgold_initial");

                break;
            case ComplexActionHelper.WithdrawGoldResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}