#region
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts.GuildBank;

public class GuildDepositItemScript : DialogScriptBase
{
    private readonly ILogger<GuildDepositItemScript> Logger;
    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;

    /// <inheritdoc />
    public GuildDepositItemScript(
        Dialog subject,
        ILogger<GuildDepositItemScript> logger,
        IStorage<GuildHouseState> guildHouseStateStorage
    )
        : base(subject)
    {
        Logger = logger;
        GuildHouseStateStorage = guildHouseStateStorage;
    }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_depositguilditem_initial":
            {
                OnDisplayingInitial(source);

                break;
            }
            case "generic_depositguilditem_amountrequest":
            {
                OnDisplayingAmountRequest(source);

                break;
            }
        }
    }

    private void OnDisplayingAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte>(out var slot) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var total = source.Inventory.CountOf(item.DisplayName);

        if (total == 1)
        {
            Subject.MenuArgs.Add("1");

            Subject.Next(source);

            return;
        }

        Subject.InjectTextParameters(item.DisplayName, total);
    }

    private void OnDisplayingInitial(Aisling source) => Subject.Slots = source.Inventory.Where(obj => obj.Template is { AccountBound: false, NoTrade: false })
                                                                              .Select(obj => obj.Slot)
                                                                              .ToList();

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_depositguilditem_amountrequest":
            {
                OnNextAmountRequest(source);

                break;
            }
        }
    }

    private void OnNextAmountRequest(Aisling source)
    {
        if (!TryFetchArgs<byte, int>(out var slot, out var amount) || (amount <= 0) || !source.Inventory.TryGetObject(slot, out var item))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (source.Guild == null)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var depositItemResult = ComplexActionHelper.DepositGuildItem(source, slot, amount);

        switch (depositItemResult)
        {
            case ComplexActionHelper.DepositItemResult.Success:
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Item, Topics.Actions.Deposit)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(item)
                      .WithProperty(source.Guild)
                      .LogInformation(
                          "Aisling {@AislingName} deposited {Amount} {@ItemName} in the {GuildName} bank",
                          source.Name,
                          amount,
                          item.DisplayName,
                          source.Guild.Name);
                
                var guildHouseState = GuildHouseStateStorage.Value;
                guildHouseState.SetStorage(GuildHouseStateStorage);
                
                guildHouseState.LogItemTransaction(source.Guild.Name, source.Name, item.DisplayName, GuildHouseState.TransactionType.Deposit);
                return;
            
            case ComplexActionHelper.DepositItemResult.ItemAccountBound:
            {
                Subject.Reply(source, "You cannot deposit that item into a guild bank.", "generic_depositguilditem_initial");  
            }

                return;
            case ComplexActionHelper.DepositItemResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have that many.", "generic_depositguilditem_initial");

                return;
      
            case ComplexActionHelper.DepositItemResult.NotEnoughGold:
                //Subject.Reply(source, $"You don't have enough gold, you need {}");
                //this script doesnt currently take into account deposit fees

                return;
            case ComplexActionHelper.DepositItemResult.ItemDamaged:
                Subject.Reply(source, "That item is damaged, I don't want to be responsible for it.", "generic_depositguilditem_initial");

                return;
            case ComplexActionHelper.DepositItemResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(depositItemResult), depositItemResult, null);
        }
    }
}