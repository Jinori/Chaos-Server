using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Definitions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusCheckEffectsScript : DialogScriptBase
{
    /// <inheritdoc />
    public TerminusCheckEffectsScript(Dialog subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDisplaying(Aisling source)
    {
        source.Trackers.Enums.TryGetValue(out TutorialQuestStage tutorial);

        if (tutorial != TutorialQuestStage.CompletedTutorial)
            return;

        switch (Subject.Template.TemplateKey.ToLower())
        {
            case "terminus_initial":
            {
                var option = new DialogOption
                {
                    DialogKey = "terminus_checkStatsInitial",
                    OptionText = "Check Stats"
                };

                if (!Subject.HasOption(option.OptionText))
                    Subject.Options.Add(option);

                break;
            }

            case "terminus_checkeffects":
            {
                var sb = new StringBuilder();

                sb.AppendLineFColored(MessageColor.Yellow, "Current Effects: ", MessageColor.Gray);

                foreach (var eff in source.Effects
                                          .OrderBy(x => x.Remaining)
                                          .ToList())
                {
                    sb.Append('\n');
                    sb.Append($"{eff.Name}: {eff.Remaining.Humanize()} remaining.");
                }

                source.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
                Subject.Close(source);

                break;
            }
        }
    }
}