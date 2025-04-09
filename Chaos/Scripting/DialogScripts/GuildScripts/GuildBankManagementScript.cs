#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildBankManagementScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildBankManagementScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildManagementScript> logger)
        : base(
            subject,
            clientRegistry,
            guildStore,
            guildFactory,
            logger) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        //ensure the player is still in a guild
        if (!IsInGuild(source, out _, out var sourceRank))
            Subject.AddOption("I must've fallen.", "Close");
        else if (sourceRank.IsLeaderRank)
            Subject.AddOptions(
                ("Deposit Item", "generic_depositguilditem_initial"),
                ("Withdraw Item", "generic_withdrawguilditem_initial"),
                ("Deposit Gold", "generic_depositguildgold_initial"),
                ("Withdraw Gold", "generic_withdrawguildgold_initial"),
                ("View Bank", "generic_viewbank_initial"),
                ("Nevermind", "aricin_initial"));
        else if (sourceRank.IsOfficerRank)
            Subject.AddOptions(
            ("Deposit Item", "generic_depositguilditem_initial"),
        ("Withdraw Item", "generic_withdrawguilditem_initial"),
        ("Deposit Gold", "generic_depositguildgold_initial"),
        ("Withdraw Gold", "generic_withdrawguildgold_initial"),
                ("View Bank", "generic_viewbank_initial"),
                ("Nevermind", "aricin_initial"));
        else
            Subject.AddOptions(("Deposit Item", "generic_depositguilditem_initial"),("Deposit Gold", "generic_depositguildgold_initial"), ("View Bank", "generic_viewbank_initial"),  ("Nevermind", "aricin_initial"));
    }
}