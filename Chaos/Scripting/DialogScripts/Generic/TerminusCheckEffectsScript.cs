using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
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
                    sb.AppendLineF();
                    sb.Append($"{eff.Name}: {eff.Remaining.Humanize()} remaining.");
                }

                sb.AppendLineF();
                sb.AppendLineFColored(MessageColor.Yellow, "Set Bonuses: ", MessageColor.Gray);

                var setBonusItems = source.Equipment
                                       .Where(item => item.Script.Is<SetBonusItemScriptBase>())
                                       .ToList();

                var setBonusDetails = setBonusItems.GroupBy(item => item.Script.As<SetBonusItemScriptBase>())
                                                   .Select(
                                                       itemGroup =>
                                                       {
                                                           var setBonusScript = itemGroup.Key;

                                                           return new
                                                           {
                                                               SetBonusName = setBonusScript!.ScriptKey,
                                                               NumItems = itemGroup.Count(),
                                                               MaxItems = setBonusScript.SetBonus.Keys.Max(),
                                                               CurrentBonus = setBonusScript.GetCumulativeBonus(itemGroup.Count())
                                                           };
                                                       })
                                                   .ToList();
                
                foreach(var details in setBonusDetails)
                {
                    sb.AppendLineF($"{details.SetBonusName}: {details.NumItems}/{details.MaxItems}");
                    var setBonusStats = details.CurrentBonus.ToString();
                    var lines = setBonusStats.Split('\n');
                    
                    //indent the stats for the set bonus
                    foreach (var line in lines)
                        sb.AppendLineF($"  {line}");
                }
                
                source.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
                Subject.Close(source);

                break;
            }
        }
    }
}