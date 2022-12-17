using Chaos.Clients.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Networking;
using Chaos.Networking.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts.Guild
{
    public class GuildRequestAcceptedScript : DialogScriptBase
    {
        private readonly IClientRegistry<IWorldClient> ClientRegistry;

        public GuildRequestAcceptedScript(Dialog subject, IClientRegistry<IWorldClient> clientRegistry) : base(subject)
        {
            ClientRegistry = clientRegistry;
        }

        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            var match = Regex.Match(Subject.Text, @"Do you accept your invitation to the (.+?) guild").Groups[1].Value;
            if (optionIndex is 1)
            {
                source.GuildName = match;
                source.GuildTitle = "Member";
                source.Titles.Add(match);
                source.Client.SendServerMessage(Common.Definitions.ServerMessageType.GuildChat, $"You have joined a guild!");
                source.Legend.AddOrAccumulate(new Objects.Legend.LegendMark($"Joined {match} guild", "guild", Common.Definitions.MarkIcon.Yay, Common.Definitions.MarkColor.LightGreen, 1, Time.GameTime.Now));
            }

            var guildmates = ClientRegistry.Where(a => a != null && a.Aisling.GuildName!.EqualsI(match));

            foreach (var guildmate in guildmates)
            {
                guildmate.Aisling.Client.SendServerMessage(Common.Definitions.ServerMessageType.GuildChat, $"Welcome {source.Name} to {match}!");
            }

            Subject.Close(source);
        }
    }
}
