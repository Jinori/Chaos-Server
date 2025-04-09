#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Chaos.Scripting.DialogScripts.GuildScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Utilities;
#endregion

namespace Chaos.Scripting.DialogScripts.GuildScripts;

public class GuildMemberAdmitScript : GuildScriptBase
{
    private readonly IDialogFactory DialogFactory;

    public GuildMemberAdmitScript(
        Dialog subject,
        IClientRegistry<IChaosWorldClient> clientRegistry,
        IStore<Guild> guildStore,
        IFactory<Guild> guildFactory,
        ILogger<GuildMemberAdmitScript> logger,
        IDialogFactory dialogFactory)
        : base(
            subject,
            clientRegistry,
            guildStore,
            guildFactory,
            logger)
        => DialogFactory = dialogFactory;

    public override void OnDisplaying(Aisling source)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_guild_members_admit_confirmation":
            {
                OnDisplayingConfirmation(source);

                break;
            }
            case "generic_guild_members_admit_accepted":
            {
                OnDisplayingAccepted(source);

                break;
            }
            case "generic_guild_members_player_confirmation":
            {
                OnDisplayingPlayerConfirmation(source);

                break;
            }
        }
    }

    private void OnDisplayingAccepted(Aisling source)
    {
        if (!IsInGuild(source, out var guild, out var sourceRank))
        {
            Subject.Reply(source, "You are not in a guild", "top");

            return;
        }

        if (!sourceRank.IsOfficerRank)
        {
            Subject.Reply(source, "You do not have permission to admit members", "generic_guild_members_initial");

            return;
        }

        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var aislingToAdmit = source.MapInstance
                                   .GetEntities<Aisling>()
                                   .FirstOrDefault(aisling => aisling.Name.EqualsI(name));

        if (aislingToAdmit is null)
        {
            Subject.Reply(source, $"{name} is not nearby", "generic_guild_members_initial");

            return;
        }

        if ((aislingToAdmit.MapInstance.Name != "Abel Tavern") && 
            (aislingToAdmit.MapInstance.LoadedFromInstanceId != "guildhallmain"))
        {
            Subject.Reply(
                source,
                $"{name} is not in the tavern to be admitted. To add Aislings to your guild register, I need them to be present.",
                "generic_guild_members_initial");

            return;
        }


        if (aislingToAdmit.Name == source.Name)
        {
            Subject.Reply(source, "You cannot admit yourself into a guild.");

            return;
        }

        if (IsInGuild(aislingToAdmit, out _, out _))
        {
            Subject.Reply(source, $"{name} is already in a guild", "generic_guild_members_initial");

            return;
        }

        // Pass the inviting player's name as an argument
        var dialog = DialogFactory.Create("generic_guild_members_player_confirmation", Subject.DialogSource);
        dialog.MenuArgs = Subject.MenuArgs;
        dialog.MenuArgs.Add(source.Name); // Passing both the invited player's name and the inviting player's name
        dialog.InjectTextParameters(source.Name, source.Guild!.Name);
        dialog.Display(aislingToAdmit);
        Subject.Close(source);
    }

    private void OnDisplayingConfirmation(Aisling source)
    {
        if (!TryFetchArgs<string>(out var name) || string.IsNullOrEmpty(name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        Subject.InjectTextParameters(name);
    }

    private void OnDisplayingPlayerAccepted(Aisling source, byte? optionIndex)
    {
        if (!Subject.MenuArgs.TryGet<string>(0, out var playerInvitedName))
        {
            source.SendOrangeBarMessage(DialogString.UnknownInput);
            Subject.Close(source);

            return;
        }

        // Retrieve the inviting player's name from MenuArgs
        if (!Subject.MenuArgs.TryGet<string>(1, out var invitingPlayerName))
        {
            source.SendOrangeBarMessage(DialogString.UnknownInput);
            Subject.Close(source);

            return;
        }

        var invitingPlayer = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(invitingPlayerName));

        if ((invitingPlayer == null) || !invitingPlayer.Aisling.OnSameMapAs(source))
        {
            source.SendOrangeBarMessage("It does not look like they are here.");
            Subject.Close(source);

            return;
        }

        if (!IsInGuild(invitingPlayer.Aisling, out var guild, out _))
        {
            Subject.Reply(source, $"{invitingPlayer.Aisling.Name} is not in a guild", "top");

            return;
        }

        var playerInvited = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(playerInvitedName));

        if ((playerInvited == null) || !playerInvited.Aisling.OnSameMapAs(source))
        {
            source.SendOrangeBarMessage("It does not look like they are here.");
            Subject.Close(source);

            return;
        }

        if (optionIndex is 2)
        {
            invitingPlayer.Aisling.SendOrangeBarMessage($"{playerInvited.Aisling.Name} has rejected your guild invite.");
            playerInvited.Aisling.SendOrangeBarMessage($"You denied {invitingPlayer.Aisling.Name}'s invite to join {guild.Name}");
        }

        if (optionIndex is 1)
        {
            Logger.WithTopics(Topics.Entities.Guild, Topics.Actions.Join)
                  .WithProperty(Subject)
                  .WithProperty(Subject.DialogSource)
                  .WithProperty(invitingPlayer.Aisling.Name)
                  .WithProperty(guild)
                  .WithProperty(playerInvited.Aisling.Name)
                  .LogInformation(
                      "Aisling {@AislingName} admitted {@TargetAislingName} to {@GuildName}",
                      invitingPlayer.Aisling.Name,
                      playerInvited.Aisling.Name,
                      guild.Name);

            invitingPlayer.Aisling.SendOrangeBarMessage($"{playerInvited.Aisling.Name} has joined {guild.Name}!");
            playerInvited.Aisling.SendOrangeBarMessage($"You joined {guild.Name}!");
            guild.AddMember(playerInvited.Aisling, invitingPlayer.Aisling);

            GuildStore.Save(guild);
        }

        Subject.Close(source);
    }

    private void OnDisplayingPlayerConfirmation(Aisling source)
    {
        if (!Subject.MenuArgs.TryGet<string>(1, out var name))
        {
            Subject.ReplyToUnknownInput(source);

            return;
        }

        var playerInvited = ClientRegistry.FirstOrDefault(cli => cli.Aisling.Name.EqualsI(name));

        if ((playerInvited == null) || !playerInvited.Aisling.OnSameMapAs(source))
            Subject.Reply(source, "The player who invited you had left.");
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "generic_guild_members_player_confirmation":
            {
                OnDisplayingPlayerAccepted(source, optionIndex);

                break;
            }
        }
    }
}