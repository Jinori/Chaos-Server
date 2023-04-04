using Chaos.Extensions.Common;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Guild;

public class GuildAddScript : DialogScriptBase
{
    private readonly InputCollector InputCollector;
    private string? GuildMemberSelected;
    private IDialogFactory DialogFactory { get; }
    private Aisling? GuildAdd { get; set; }
    private IMerchantFactory MerchantFactory { get; }

    public GuildAddScript(Dialog subject, IDialogFactory dialogFactory, IMerchantFactory merchantFactory)
        : base(subject)
    {
        InputCollector = new InputCollectorBuilder().RequestOptionSelection(
                                                        DialogString.From(() => $"{GuildMemberSelected} will be added. Is this correct?"),
                                                        DialogString.Yes,
                                                        DialogString.No)
                                                    .HandleInput(HandleConfirmation)
                                                    .Build();

        DialogFactory = dialogFactory;
        MerchantFactory = merchantFactory;
    }

    public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (option is not 1)
            return false;

        var npcpoint = new Point(aisling.X, aisling.Y);
        var merchant = MerchantFactory.Create("aricin", aisling.MapInstance, npcpoint);
        var dialogNew = DialogFactory.Create("aricin_acceptGuild", merchant);

        dialogNew.Text = $"Do you accept your invitation to the {aisling.GuildName} guild?");
        dialogNew.Display(GuildAdd!);

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

            GuildMemberSelected = guildMember;
            var point = new Point(source.X, source.Y);
            var member = source.MapInstance.GetEntitiesWithinRange<Aisling>(point).FirstOrDefault(x => x.Name.EqualsI(GuildMemberSelected));

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

            GuildAdd = member;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}