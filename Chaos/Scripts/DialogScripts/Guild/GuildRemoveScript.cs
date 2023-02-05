using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripts.DialogScripts.Guild;

public class GuildRemoveScript : DialogScriptBase
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly InputCollector InputCollector;
    private string? GuildMemberSelected;
    private Aisling? GuildRemove { get; set; }

    public GuildRemoveScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry)
        : base(subject)
    {
        InputCollector = new InputCollectorBuilder().RequestOptionSelection(
                                                        DialogString.From(() => $"{GuildMemberSelected} will be removed. Is this correct?"),
                                                        DialogString.Yes,
                                                        DialogString.No)
                                                    .HandleInput(HandleConfirmation)
                                                    .Build();

        ClientRegistry = clientRegistry;
    }

    public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (option is not 1)
            return false;

        var guildmates = ClientRegistry.Where(a => a.Aisling.GuildName!.EqualsI(GuildRemove?.GuildName!));

        foreach (var guildmate in guildmates)
            guildmate.Aisling.Client.SendServerMessage(
                ServerMessageType.GuildChat,
                $"{GuildRemove!.Name} has been removed from {GuildRemove!.GuildName}");

        GuildRemove!.GuildName = null;
        GuildRemove!.GuildTitle = null;
        GuildRemove!.Legend.Remove("guild", out _);

        dialog.Close(aisling);

        return true;
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (GuildMemberSelected == null)
        {
            if (!Subject.MenuArgs.TryGet<string>(0, out var guildMember))
            {
                Subject.Reply(source, "That name cannot be used.");

                return;
            }

            if (guildMember.Length > 13)
            {
                Subject.Reply(source, "That name is too long.");

                return;
            }

            GuildMemberSelected = guildMember;

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            var player = ClientRegistry
                         .Select(c => c.Aisling)
                         .Where(a => a != null)
                         .FirstOrDefault(a => a.Name.EqualsI(GuildMemberSelected));

            if (player == null)
            {
                source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{GuildMemberSelected} is not online");

                return;
            }

            if (player.GuildName is not null && !player.GuildName.EqualsI(source.GuildName!))
            {
                Subject.Reply(source, "That aisling does not belong to your guild.");

                return;
            }

            GuildRemove = player;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}