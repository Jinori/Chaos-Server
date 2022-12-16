using Chaos.Common.Definitions;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts
{
    public class GuildCreationScript : DialogScriptBase
    {
        private readonly InputCollector inputCollector;
        private string? guildName;

        public GuildCreationScript(Dialog subject) : base(subject)
        {
            inputCollector = new InputCollectorBuilder().RequestOptionSelection(DialogString.From(() => $"{guildName} has been chosen. Is this correct?"), DialogString.Yes, DialogString.No)
            .HandleInput(HandleConfirmation).Build();
        }


        public override void OnNext(Aisling source, byte? optionIndex = null)
        {
            if (guildName == null)
            {
                if (!Subject.MenuArgs.TryGet<string>(0, out var guildname))
                {
                    Subject.Reply(source, "That guild name cannot be used.");
                    return;
                }
                if (guildname.Length > 18)
                {
                    Subject.Reply(source, "This guild name is too long.");
                    return;
                }
                if (guildname == null)
                {
                    Subject.Reply(source, DialogString.UnknownInput.Value);
                    return;
                }
                guildName = guildname;
            }
            inputCollector.Collect(source, Subject, optionIndex);
        }

        public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
        {
            if (option is not 1)
            {
                return false;
            }
            aisling.GuildName = guildName;
            aisling.GuildTitle = "Leader";
            aisling.Titles.Add(aisling.GuildName!);
            aisling.Client.SendServerMessage(ServerMessageType.GuildChat, $"[Aricin] Welcome to your new guild, {guildName}.");
            aisling.Legend.AddOrAccumulate(new Objects.Legend.LegendMark($"Leader of {guildName} guild", "guild", MarkIcon.Yay, MarkColor.LightGreen, 1, Time.GameTime.Now));

            dialog.Close(aisling);
            return true;
        }
    }
}
