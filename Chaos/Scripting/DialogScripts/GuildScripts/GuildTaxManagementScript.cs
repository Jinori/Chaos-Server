#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildTaxManagementScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildTaxManagementScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildTaxManagementScript> logger)
        : base(
            subject,
            clientRegistry,
            guildStore,
            guildFactory,
            logger) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_guild_tax_initial":
            case "generic_guild_tax_changerate": 
            case "generic_guild_tax_change":
            {
                if (source.Guild != null) 
                    Subject.InjectTextParameters(source.Guild.TaxRatePercent);
                
                break;
            }
        }
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (Subject.Template.TemplateKey.ToLower() is "generic_guild_tax_changerate")
        {
            if (IsInGuild(source, out _, out var sourceRank) && sourceRank.IsLeaderRank)
            {
                switch (optionIndex - 1)
                {
                    case 1:
                        source.Guild?.SetTaxRate(5);
                        NotifyGuildMembersofRateChange(source, 5);
                        break;
                    
                    case 2:
                        source.Guild?.SetTaxRate(10);
                        NotifyGuildMembersofRateChange(source, 10);
                        break;
                    
                    case 3:
                        source.Guild?.SetTaxRate(15);
                        NotifyGuildMembersofRateChange(source, 15);
                        break;
                    
                    case 4:
                        source.Guild?.SetTaxRate(20);
                        NotifyGuildMembersofRateChange(source, 20);
                        break;
                    
                    case 5:
                        source.Guild?.SetTaxRate(25);
                        NotifyGuildMembersofRateChange(source, 25);
                        break;
                }
            }
            
        }
    }

    private void NotifyGuildMembersofRateChange(Aisling source, int taxRatePercent)
    {
        if (source.Guild != null)
            foreach (var member in source.Guild.GetOnlineMembers())
                member.SendServerMessage(ServerMessageType.GuildChat, $"Leader {source.Name} has selected a {taxRatePercent}% tax rate for {source.Guild.Name}");
    }
}