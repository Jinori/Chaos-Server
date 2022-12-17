using Chaos.Clients;
using Chaos.Clients.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Networking.Options;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Servers;
using Chaos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts.Guild
{
    public class GuildRemoveScript : DialogScriptBase
    {
        private readonly InputCollector inputCollector;
        private string? guildMemberSelected;
        private readonly IClientRegistry<IWorldClient> ClientRegistry;
        private Aisling? guildRemove { get; set; }

        public GuildRemoveScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry) : base(subject)
        {
            inputCollector = new InputCollectorBuilder().RequestOptionSelection(DialogString.From(() => $"{guildMemberSelected} will be removed. Is this correct?"), DialogString.Yes, DialogString.No)
.HandleInput(HandleConfirmation).Build();
            ClientRegistry = clientRegistry;
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (guildMemberSelected == null)
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var guildMember))
                {
                    Subject.Reply(source, "That name cannot be used.");
                    return;
                }
                if (guildMember.Length > 18)
                {
                    Subject.Reply(source, "That name is too long.");
                    return;
                }
                if (guildMember == null)
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);
                    return;
                }
                guildMemberSelected = guildMember;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                var player = ClientRegistry
                             .Select(c => c.Aisling)
                             .Where(a => a != null)
                             .FirstOrDefault(a => a.Name.EqualsI(guildMemberSelected));

                if (player == null)
                {
                    source.Client.SendServerMessage(ServerMessageType.OrangeBar1, $"{guildMemberSelected} is not online");
                    return;
                }
                if (player.GuildName is not null && !player.GuildName.EqualsI(source.GuildName!))
                {
                    Subject.Reply(source, "That aisling does not belong to your guild.");
                    return;
                }
                guildRemove = player;
            }
            inputCollector.Collect(source, Subject, optionIndex);
        }

        public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
        {
            if (option is not 1)
            {
                return false;
            }

            var guildmates = ClientRegistry.Where(a => a != null && a.Aisling.GuildName!.EqualsI(guildRemove.GuildName!));

            foreach (var guildmate in guildmates)
            {
                guildmate.Aisling.Client.SendServerMessage(ServerMessageType.GuildChat, $"{guildRemove.Name} has been removed from {guildRemove.GuildName}");
            }

            guildRemove!.GuildName = null;
            guildRemove!.GuildTitle = null;
            guildRemove!.Legend.Remove("guild", out var mark);



            dialog.Close(aisling);
            return true;
        }

    }
}
