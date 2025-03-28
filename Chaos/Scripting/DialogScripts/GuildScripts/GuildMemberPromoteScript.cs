#region
using System.Diagnostics;
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Storage.Abstractions;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildMemberPromoteScript : GuildScriptBase
{
    /// <inheritdoc />
    public GuildMemberPromoteScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildMemberPromoteScript> logger)
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
            //occurs after you choose a guild member to promote
            case "generic_guild_members_promote_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }

            case "generic_guild_members_transferownership_confirmation":
            {
                OnDisplayingConfirmationTransfer(source);
                break;
            }

            case "generic_guild_members_transferownership_accepted":
            {
                OnDisplayingAcceptedTransfer(source);
                break;
            }

            //occurs after you confirm the guild member to promote
            case "generic_guild_members_promote_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        //ensure the player is in a guild
        if (!IsInGuild(source, out var guild, out var sourceRank))
        {
            Subject.Reply(source, "You are not in a guild.", "top");

            return;
        }

        //ensure the player name is present and not empty
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the player has permission to promote members
        if (!sourceRank.IsOfficerRank)
        {
            Subject.Reply(source, "You do not have permission to promote members.", "generic_guild_members_initial");

            return;
        }

        //ensure the player to promote is in the guild
        if (!guild.HasMember(name))
        {
            Subject.Reply(source, $"{name} is not in your guild.", "generic_guild_members_initial");

            return;
        }
        
        if (source.Name.EqualsI(name))
        {
            Subject.Reply(source, "You dumbly point to yourself.", "generic_guild_members_initial");

            return;
        }

        var targetCurrentRank = guild.RankOf(name);
        //leader can promote anyone that is currently 1 tier higher than them (can promote TO their own tier, from 1 rank away)
        //officer can promote anyone that is currently 2 tiers higher than them (can promote TO 1 tier higher than them, from 2 ranks away)
        var factor = sourceRank.IsLeaderRank ? 1 : 2;
        //ensure the player to promote is not the same or higher rank (same or lower tier)
        if (!sourceRank.IsSuperiorTo(targetCurrentRank, factor))
        {
            Subject.Reply(source, $"You do not have permission to promote {name}.", "generic_guild_members_initial");

            return;
        }
        
        if(!targetCurrentRank.CanBePromoted)
        {
            Subject.Reply(source, $"{name} cannot be promoted any further.", "generic_guild_members_initial");

            return;
        }
        
        guild.ChangeRank(name, targetCurrentRank.Tier - 1, source);
        
        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Promote)
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .LogInformation(
                  "Aisling {@AislingName} promoted {@TargetAislingName} to {@RankName} in {@GuildName}",
                  source.Name,
                  name,
                  sourceRank.Name,
                  guild.Name);
    }
    
    private void OnDisplayingAcceptedTransfer(Aisling source)
    {
        //ensure the player is in a guild
        if (!IsInGuild(source, out var guild, out var sourceRank))
        {
            Subject.Reply(source, "You are not in a guild.", "top");

            return;
        }

        //ensure the player name is present and not empty
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the player has permission to transfer ownership
        if (!sourceRank.IsLeaderRank)
            Subject.Reply(source, "You do not have permission to transfer ownership.", "generic_guild_members_initial");

        //ensure the player to promote is in the guild
        if (!guild.HasMember(name))
        {
            Subject.Reply(source, $"{name} is not in your guild.", "generic_guild_members_initial");

            return;
        }

        if (source.Name.EqualsI(name))
        {
            Subject.Reply(source, "You dumbly point to yourself.", "generic_guild_members_initial");

            return;
        }

        var targetCurrentRank = guild.RankOf(name);
        
        //ensure the player to promote is not the same or higher rank (same or lower tier)
        if (!sourceRank.IsSuperiorTo(targetCurrentRank))
        {
            Subject.Reply(source, $"You do not have permission to transfer ownership to {name}.", "generic_guild_members_initial");

            return;
        }
        
        guild.ChangeRank(name, 0, source);
        guild.ChangeRank(source.Name, 1, source);
        source.SendOrangeBarMessage($"You give leadership over to {name}.");

        foreach (var onlineMember in guild.GetOnlineMembers())
            onlineMember.SendOrangeBarMessage($"{source.Name} has transferred leadership to {name}. Long live {name}!");
        
        Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Promote)
              .WithProperty(Subject)
              .WithProperty(Subject.DialogSource)
              .WithProperty(source)
              .WithProperty(guild)
              .LogInformation(
                  "Aisling {@AislingName} promoted {@TargetAislingName} to {@RankName} in {@GuildName}",
                  source.Name,
                  name,
                  sourceRank.Name,
                  guild.Name);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        //ensure the player is in a guild
        if (!IsInGuild(source, out var guild, out _))
        {
            Subject.Reply(source, "You are not in a guild.", "top");

            return;
        }

        //ensure the player name is present and not empty
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the player to promote is in the guild
        if (!guild.HasMember(name))
        {
            Subject.Reply(source, $"{name} is not in your guild.", "generic_guild_members_initial");

            return;
        }

        var targetCurrentRank = guild.RankOf(name);
        
        //grab the rank the aisling will be promoted to
        if (!guild.TryGetRank(targetCurrentRank.Tier - 1, out var toRank))
            throw new UnreachableException(
                "This should only occur if the guild does not have the rank tier - 1, but that should be checked already");
        
        Subject.InjectTextParameters(name, toRank.Name);
    }
    
    private void OnDisplayingConfirmationTransfer(Aisling source)
    {
        //ensure the player is in a guild
        if (!IsInGuild(source, out var guild, out _))
        {
            Subject.Reply(source, "You are not in a guild.", "top");

            return;
        }

        //ensure the player name is present and not empty
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        //ensure the player to promote is in the guild
        if (!guild.HasMember(name))
        {
            Subject.Reply(source, $"{name} is not in your guild.", "generic_guild_members_initial");

            return;
        }
        
        //grab the rank the aisling will be promoted to
        if (!guild.TryGetRank(0, out var toRank))
            throw new UnreachableException(
                "This should only occur if the guild does not have the rank tier - 1, but that should be checked already");

        Subject.InjectTextParameters(name, toRank.Name);
    }
}