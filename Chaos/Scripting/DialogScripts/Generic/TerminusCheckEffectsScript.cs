using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.WorldScripts.WorldBuffs.Guild;
using Chaos.Scripting.WorldScripts.WorldBuffs.Religion;
using Chaos.Storage.Abstractions;
using Humanizer;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusCheckEffectsScript : DialogScriptBase
{
    private readonly IStorage<GuildBuffs> GuildBuffStorage;
    private readonly IStorage<ReligionBuffs> ReligionBuffStorage;

    /// <inheritdoc />
    public TerminusCheckEffectsScript(Dialog subject, IStorage<GuildBuffs> guildBuffStorage, IStorage<ReligionBuffs> religionBuffStorage)
        : base(subject)
    {
        ReligionBuffStorage = religionBuffStorage;
        GuildBuffStorage = guildBuffStorage;   
    }

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

                //EFFECTS
                sb.AppendLineFColored(MessageColor.Yellow, "Current Effects: ", MessageColor.Gray);

                foreach (var eff in source.Effects
                                          .OrderBy(x => x.Remaining)
                                          .ToList())
                {
                    sb.AppendLineF();
                    sb.Append($"{eff.Name}: {eff.Remaining.Humanize()} remaining.");
                }
                
                sb.AppendLineF();
                
                //SET BONUSES
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
                
                sb.AppendLineF();
                
                //GUILD BUFFS
                sb.AppendLineFColored(MessageColor.Yellow, "Guild Buffs: ", MessageColor.Gray);

                if (source.Guild != null)
                {
                    var activeGuildBuffs = GuildBuffStorage.Value
                                                           .ActiveBuffs
                                                           .Where(buff => buff.GuildName.EqualsI(source.Guild.Name))
                                                           .OrderBy(buff => buff.Duration - buff.Elapsed)
                                                           .ToList();

                    foreach (var buff in activeGuildBuffs)
                        sb.AppendLineF($"{buff.BuffName}: {(buff.Duration - buff.Elapsed).Humanize()} remaining");
                }
                
                sb.AppendLineF();
                
                //RELIGION BUFFS
                sb.AppendLineFColored(MessageColor.Yellow, "Religion Buffs: ", MessageColor.Gray);


                    var activeReligionBuffs = ReligionBuffStorage.Value
                                                           .ActiveBuffs
                                                           .OrderBy(buff => buff.Duration - buff.Elapsed)
                                                           .ToList();

                    foreach (var buff in activeReligionBuffs)
                        sb.AppendLineF($"{buff.BuffName}: {(buff.Duration - buff.Elapsed).Humanize()} remaining");
                
                
                source.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
                Subject.Close(source);

                break;
            }
        }
    }
}