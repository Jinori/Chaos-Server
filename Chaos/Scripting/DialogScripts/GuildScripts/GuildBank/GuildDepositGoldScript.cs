#region
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.DialogScripts.BankScripts;
using Chaos.Storage.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts.GuildBank;

public class GuildDepositGoldScript : DialogScriptBase
{
    private readonly ILogger<GuildDepositGoldScript> Logger;
    private readonly IStorage<GuildHouseState> GuildHouseStateStorage;

    /// <inheritdoc />
    public GuildDepositGoldScript(Dialog subject, ILogger<GuildDepositGoldScript> logger, IStorage<GuildHouseState> guildHouseStateStorage)
        : base(subject)
    {
        Logger = logger;
        GuildHouseStateStorage = guildHouseStateStorage;
    } 

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source) => Subject.InjectTextParameters(source.Gold);

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (!TryFetchArgs<int>(out var amount) || (amount <= 0))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (source.Guild == null)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }
        
        var depositResult = ComplexActionHelper.DepositGuildGold(source, amount);

        switch (depositResult)
        {
            case ComplexActionHelper.DepositGoldResult.Success:
                Logger.WithTopics(Topics.Entities.Aisling, Topics.Entities.Gold, Topics.Actions.Deposit)
                      .WithProperty(Subject)
                      .WithProperty(Subject.DialogSource)
                      .WithProperty(source)
                      .WithProperty(source.Guild)
                      .LogInformation("Aisling {@AislingName} deposited {Amount} gold in the {GuildName} bank", source.Name, amount, source.Guild.Name);
                
                var guildHouseState = GuildHouseStateStorage.Value;
                guildHouseState.SetStorage(GuildHouseStateStorage);
                
                guildHouseState.LogItemTransaction(source.Guild.Name, source.Name, amount.ToString(), GuildHouseState.TransactionType.GoldDeposit);

                break;
            case ComplexActionHelper.DepositGoldResult.DontHaveThatMany:
                Subject.Reply(source, "You don't have enough gold.", "generic_depositguildgold_initial");

                break;
            case ComplexActionHelper.DepositGoldResult.BadInput:
                Subject.ReplyToUnknownInput(source);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}