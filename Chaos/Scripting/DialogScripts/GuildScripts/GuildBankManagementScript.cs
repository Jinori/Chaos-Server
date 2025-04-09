using System.Text;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildBankManagementScript(
    Dialog subject,
    IClientRegistry<IChaosWorldClient> clientRegistry,
    IStore<Guild> guildStore,
    IFactory<Guild> guildFactory,
    ILogger<GuildBankManagementScript> logger,
    IStorage<GuildHouseState> guildHouseStateStorage
)
    : GuildScriptBase(
        subject,
        clientRegistry,
        guildStore,
        guildFactory,
        logger)
{
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "stash_guildbank":
            {
                if (!IsInGuild(source, out _, out var sourceRank))
                {
                    Subject.AddOption("I must've fallen.", "Close");
                    break;
                }

                if (sourceRank.IsLeaderRank || sourceRank.IsOfficerRank)
                {
                    Subject.AddOptions(
                        ("Deposit Item", "generic_depositguilditem_initial"),
                        ("Withdraw Item", "generic_withdrawguilditem_initial"),
                        ("Deposit Gold", "generic_depositguildgold_initial"),
                        ("Withdraw Gold", "generic_withdrawguildgold_initial"),
                        ("View Bank", "generic_viewbank_initial"),
                        ("View Logs", "stash_guildbanklogs"),
                        ("Nevermind", "aricin_initial"));
                }
                else
                {
                    Subject.AddOptions(
                        ("Deposit Item", "generic_depositguilditem_initial"),
                        ("Deposit Gold", "generic_depositguildgold_initial"),
                        ("View Bank", "generic_viewbank_initial"),
                        ("View Logs", "stash_guildbanklogs"),
                        ("Nevermind", "aricin_initial"));
                }

                break;
            }

            case "stash_guildbankwithdrawlog":
            {
                var guildHouseState = guildHouseStateStorage.Value;
                guildHouseState.SetStorage(guildHouseStateStorage);

                if (!guildHouseState.GuildProperties.Entries.TryGetValue(source.Guild?.Name, out var houseProperties))
                {
                    source.SendServerMessage(ServerMessageType.ScrollWindow, "No withdrawal logs found for this guild.");
                    return;
                }

                var withdrawals = houseProperties.ItemLogs
                    .Where(log => log.Action == GuildHouseState.TransactionType.Withdrawal)
                    .OrderByDescending(log => log.Timestamp)
                    .Take(30)
                    .ToList();

                if (withdrawals.Count == 0)
                {
                    source.SendServerMessage(ServerMessageType.ScrollWindow, "No recent withdrawals found.");
                    return;
                }

                ShowGuildTransactionLog(source, withdrawals, "Recent Guild Withdrawals:");
                Subject.Close(source);
                break;
            }

            case "stash_guildbankdepositlog":
            {
                var guildHouseState = guildHouseStateStorage.Value;
                guildHouseState.SetStorage(guildHouseStateStorage);

                if (!guildHouseState.GuildProperties.Entries.TryGetValue(source.Guild?.Name, out var houseProperties))
                {
                    source.SendServerMessage(ServerMessageType.ScrollWindow, "No deposit logs found for this guild.");
                    return;
                }

                var deposits = houseProperties.ItemLogs
                    .Where(log => log.Action == GuildHouseState.TransactionType.Deposit)
                    .OrderByDescending(log => log.Timestamp)
                    .Take(30)
                    .ToList();

                if (deposits.Count == 0)
                {
                    source.SendServerMessage(ServerMessageType.ScrollWindow, "No recent deposits found.");
                    return;
                }

                ShowGuildTransactionLog(source, deposits, "Recent Guild Deposits:");
                Subject.Close(source);
                break;
            }
        }
    }

    private static void ShowGuildTransactionLog(
        Aisling source,
        IEnumerable<GuildHouseState.ItemTransactionLog> logs,
        string title)
    {
        var builder = new StringBuilder();
        builder.AppendLineFColored(MessageColor.Silver, title);
        builder.AppendLine();
        builder.AppendLineFColored(MessageColor.Orange, $"{"Player",-15} {"Item",-25} {"Date",-20}");
        builder.AppendLineFColored(MessageColor.Gainsboro, new string('-', 50));

        var isSilver = true;
        foreach (var log in logs)
        {
            var color = isSilver ? MessageColor.Silver : MessageColor.Gainsboro;
            builder.AppendLineFColored(color, $"{log.Member,-15} {log.ItemName,-25} {log.Timestamp:g}");
            isSilver = !isSilver;
        }

        source.SendServerMessage(ServerMessageType.ScrollWindow, builder.ToString());
    }
}
