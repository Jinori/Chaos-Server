using Chaos.Common.Definitions;
using Chaos.Objects.Legend;
using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Time;
using Chaos.Utilities;

namespace Chaos.Scripting.DialogScripts.Guild;

public class GuildCreationScript : DialogScriptBase
{
    private readonly InputCollector InputCollector;
    private string? GuildName;

    public GuildCreationScript(Dialog subject)
        : base(subject) =>
        InputCollector = new InputCollectorBuilder().RequestOptionSelection(
                                                        DialogString.From(() => $"{GuildName} has been chosen. Is this correct?"),
                                                        DialogString.Yes,
                                                        DialogString.No)
                                                    .HandleInput(HandleConfirmation)
                                                    .Build();

    public bool HandleConfirmation(Aisling aisling, Dialog dialog, int? option = null)
    {
        if (option is not 1)
            return false;

        aisling.GuildName = GuildName;
        aisling.GuildTitle = "Leader";
        aisling.Titles.Add(aisling.GuildName!);
        aisling.Client.SendServerMessage(ServerMessageType.GuildChat, $"[Aricin] Welcome to your new guild, {GuildName}.");

        aisling.Legend.AddOrAccumulate(
            new LegendMark(
                $"Leader of {GuildName} guild",
                "guild",
                MarkIcon.Yay,
                MarkColor.LightGreen,
                1,
                GameTime.Now));

        dialog.Close(aisling);

        return true;
    }

    public override void OnNext(Aisling source, byte? optionIndex = null)
    {
        if (GuildName == null)
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

            GuildName = guildname;
        }

        InputCollector.Collect(source, Subject, optionIndex);
    }
}