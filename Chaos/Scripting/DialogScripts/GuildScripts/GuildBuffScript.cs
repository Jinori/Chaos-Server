using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Scripting.WorldScripts.WorldBuffs.Guild;
using Chaos.Storage.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildBuffScript : GuildScriptBase
{
    private readonly IStorage<GuildBuffs> GuildBuffStorage;
    private const uint GUILD_25_EXP_BUFF_COST1_HOUR = 2_000_000;
    private const uint GUILD_25_EXP_BUFF_COST3_HOUR = 5_000_000;
    private const uint GUILD_25_EXP_BUFF_COST6_HOUR = 9_000_000;
    private const uint GUILD_25_EXP_BUFF_COST12_HOUR = 16_000_000;
    private const uint GUILD_25_EXP_BUFF_COST24_HOUR = 30_000_000;
    public const string GUILD_25_EXP_BUFF_NAME = "GuildExp25";
    
    
    /// <inheritdoc />
    public GuildBuffScript(Dialog subject, IClientRegistry<IChaosWorldClient> clientRegistry, IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildBuffScript> logger,
        IStorage<GuildBuffs> guildBuffStorage)
        : base(subject, clientRegistry, guildStore,
            guildFactory,
            logger)
        => GuildBuffStorage = guildBuffStorage;

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey)
        {
            case "generic_guildbuff_25exp_confirmation":
            {
                On25ExpConfirmationDisplaying(source);
                
                break;
            }
        }
    }

    private void On25ExpConfirmationDisplaying(Aisling source)
    {
        if (Subject.Context is null)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }
        
        var choice = (byte)Subject.Context;
        var cost = GetCost(choice);
        var duration = GetDuration(choice);

        Subject.InjectTextParameters(duration.Humanize(), cost.ToString("N"));
    }

    /// <inheritdoc />
    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey)
        {
            case "generic_guildbuff_25exp_choices":
            {
                On25ExpChoicesNext(source, optionIndex);
                
                break;
            }
            case "generic_guildbuff_25exp_confirmation":
            {
                On25ExpConfirmationNext(source, optionIndex);
                
                break;
            }
        }
    }

    private void On25ExpConfirmationNext(Aisling source, byte? optionIndex = null)
    {
        if (!optionIndex.HasValue)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        if (optionIndex.Value == 2)
            return;

        if ((source.Guild == null) || Subject.Context is null)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }
        
        var rank = source.Guild.RankOf(source.Name);
        
        if(!rank.IsOfficerRank)
        {
            Subject.Reply(source, "You do not have permission to use this buff.", "top");

            return;
        }
        
        var guild = source.Guild;

        if (HasGuildBuff(guild, "GuildExp25"))
        {
            Subject.Reply(source, "Your guild already has this buff active.", "top");

            return;
        }

        var choice = (byte)Subject.Context;

        var cost = GetCost(choice);
        var duration = GetDuration(choice);

        if (!guild.Bank.RemoveGold(cost))
        {
            Subject.Reply(source, $"Your guild does not have the required {cost:N} gold.", "top");

            return;
        }

        AddGuildBuff(guild, GUILD_25_EXP_BUFF_NAME, duration);
    }

    private void On25ExpChoicesNext(Aisling source, byte? optionIndex = null)
    {
        if (!optionIndex.HasValue)
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }
        
        Subject.Context = optionIndex.Value;
    }

    public bool HasGuildBuff(Guild guild, string buffName)
    {
        if (GuildBuffStorage.Value.ActiveBuffs.Any(buff => buff.GuildName.EqualsI(guild.Name) && buff.BuffName.EqualsI(buffName)))
            return true;

        return false;
    }

    public void AddGuildBuff(Guild guild, string buffName, TimeSpan duration)
    {
        var buff = new GuildBuff
        {
            BuffName = buffName,
            Duration = duration,
            GuildName = guild.Name
        };

        GuildBuffStorage.Value.ActiveBuffs.Add(buff);

        foreach (var member in guild.GetOnlineMembers())
            member.SendActiveMessage($"{buff.BuffName} has been activated for {buff.Duration.Humanize()}");
    }
    
    private uint GetCost(byte option) => option switch
    {
        1 => GUILD_25_EXP_BUFF_COST1_HOUR,
        2 => GUILD_25_EXP_BUFF_COST3_HOUR,
        3 => GUILD_25_EXP_BUFF_COST6_HOUR,
        4 => GUILD_25_EXP_BUFF_COST12_HOUR,
        5 => GUILD_25_EXP_BUFF_COST24_HOUR,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    private TimeSpan GetDuration(byte option) => option switch
    {
        1 => TimeSpan.FromHours(1),
        2 => TimeSpan.FromHours(3),
        3 => TimeSpan.FromHours(6),
        4 => TimeSpan.FromHours(12),
        5 => TimeSpan.FromHours(24),
        _ => throw new ArgumentOutOfRangeException()
    };
}