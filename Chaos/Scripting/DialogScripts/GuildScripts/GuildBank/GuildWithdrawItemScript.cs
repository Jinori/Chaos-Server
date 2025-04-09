#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Menu;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.BankScripts;
using Chaos.Storage.Abstractions;
using Chaos.Utilities;
using Humanizer;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts.GuildBank;

public class GuildWithdrawItemScript : DialogScriptBase
{
    private readonly ILogger<GuildWithdrawItemScript> Logger;
    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;

    /// <inheritdoc />
    public GuildWithdrawItemScript(
        Dialog subject,
        ILogger<GuildWithdrawItemScript> logger,
        IStorage<GuildHouseState> guildHouseStateStorage
    )
        : base(subject)
    {
        GuildHouseStateStorage = guildHouseStateStorage;
        Logger = logger;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_withdrawguilditem_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_withdrawguilditem_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
        }
    }

    private void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<string>(out var itemName) || !TryGetItem(source, itemName, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (item.Count == 1)
        {
            Subject.MenuArgs.Add("1");
            Subject.Next(source);

            return;
        }

        Subject.InjectTextParameters(item.DisplayName, item.Count);
    }

    private void OnDisplayingInitial(Aisling source)
        => Subject.Items.AddRange(
            source.Guild!.Bank
                  .Select(ItemDetails.WithdrawItem)
                  .OrderBy(x => x.Item.Template.Category));

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_withdrawguilditem_amountrequest":
            {
                OnNextAmountRequest(source);

                break;
            }
        }
    }

    private void OnNextAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<string, int>(out var itemName, out var amount) || (amount <= 0) || !TryGetItem(source, itemName, out var item))
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

        if (!guildRank.IsOfficerRank)
        {
            Subject.Reply(source, "You are not authorized to withdraw items from the guild bank.", "top");

            return;
        }

        var withdrawResult = ComplexActionHelper.WithdrawGuildItem(source, item.DisplayName, amount);

        switch (withdrawResult)
        {
            case ComplexActionHelper.WithdrawItemResult.Success:
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Actions.Withdraw)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(item)
                      .WithProperty(source.Guild)
                      .LogInformation(
                          "Aisling {@AislingName} withdrew {Amount} {@ItemName} from the {GuildName} bank",
                          source.Name,
                          amount,
                          item.DisplayName,
                          source.Guild.Name);
                
                var guildHouseState = GuildHouseStateStorage.Value;
                guildHouseState.SetStorage(GuildHouseStateStorage);
                
                guildHouseState.LogItemTransaction(source.Guild.Name, source.Name, item.DisplayName, GuildHouseState.TransactionType.Withdrawal);

                foreach (var member in source.Guild.GetOnlineMembers())
                {
                    member.SendServerMessage(
                        ServerMessageType.GuildChat,
                        amount > 1
                            ? $"{source.Name} has withdrawn {amount} of {item.DisplayName.Pluralize()}."
                            : $"{source.Name} has withdrawn {item.DisplayName}.");
                }

                return;
            case ComplexActionHelper.WithdrawItemResult.CantCarry:
                Subject.Reply(source, "You can't carry that");

                return;
            case ComplexActionHelper.WithdrawItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many");

                return;
            case ComplexActionHelper.WithdrawItemResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(withdrawResult), withdrawResult, null);
        }
    }

    private bool TryGetItem(Aisling source, string itemName, [MaybeNullWhen(false)] out Item item)
    {
        item = source.Guild!.Bank.FirstOrDefault(obj => obj.DisplayName.Equals(itemName, StringComparison.OrdinalIgnoreCase));

        return item != null;
    }
}