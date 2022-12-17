using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Scripts.DialogScripts.Abstractions;
using Chaos.Services.Factories;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Scripts.DialogScripts.Guild
{
    public class GuildAddScript : DialogScriptBase
    {
        private readonly InputCollector inputCollector;
        private string? guildMemberSelected;
        private Aisling? guildAdd { get; set; }
        private IMerchantFactory MerchantFactory { get; init; }
        private IDialogFactory DialogFactory { get; init; }


        public GuildAddScript(Dialog subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory) : base(subject)
        {
            inputCollector = new InputCollectorBuilder().RequestOptionSelection(DialogString.From(() => $"{guildMemberSelected} will be added. Is this correct?"), DialogString.Yes, DialogString.No)
            .HandleInput(HandleConfirmation).Build();
            DialogFactory = dialogFactory;
            MerchantFactory = merchantFactory;
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
                var point = new Point(source.X, source.Y);
                var member = source.MapInstance.GetEntitiesWithinRange<Aisling>(point).FirstOrDefault(x => x.Name.EqualsI(guildMemberSelected));
                if (member is null)
                {
                    Subject.Reply(source, "That aisling is not near.");
                    return;
                }
                if (member.GuildName is not null)
                {
                    Subject.Reply(source, "That aisling already belongs to a guild.");
                    return;
                }
                guildAdd = member;
            }
            inputCollector.Collect(source, Subject, optionIndex);
        }

        public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
        {
            if (option is not 1)
            {
                return false;
            }

            var npcpoint = new Point(aisling.X, aisling.Y);
            var merchant = MerchantFactory.Create("aricin", aisling.MapInstance, npcpoint);
            var dialogNew = DialogFactory.Create("aricin_acceptGuild", merchant);

            dialogNew.Text = $"Do you accept your invitation to the {aisling.GuildName} guild?";
            dialogNew.Display(guildAdd!);

            dialog.Close(aisling);
            return true;
        }
    }
}
