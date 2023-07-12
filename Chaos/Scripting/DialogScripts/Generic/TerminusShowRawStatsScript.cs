using System.Text;
using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Menu;
using Chaos.Models.World;
using Chaos.Scripting.DialogScripts.Abstractions;

namespace Chaos.Scripting.DialogScripts.Generic;

public class TerminusShowRawStatsScript : DialogScriptBase
{
    /// <inheritdoc />
    public TerminusShowRawStatsScript(Dialog subject)
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

            case "terminus_checkrawstats":
            {
                var sb = new StringBuilder();

                sb.AppendLineFColored(MessageColor.Yellow, "Base Vitality", MessageColor.Gray);
                sb.Append($"HP: {source.StatSheet.MaximumHp} ");
                sb.AppendLine($"MP: {source.StatSheet.MaximumMp} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Base Stats", MessageColor.Gray);
                sb.Append($"STR: {source.StatSheet.Str} ");
                sb.Append($"INT: {source.StatSheet.Int} ");
                sb.Append($"WIS: {source.StatSheet.Wis} ");
                sb.Append($"CON: {source.StatSheet.Con} ");
                sb.AppendLine($"DEX: {source.StatSheet.Dex} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Base Defenses", MessageColor.Gray);
                sb.Append($"MR: {source.StatSheet.MagicResistance}% ");
                sb.AppendLine($"AC: {source.StatSheet.Ac} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Base Offenses", MessageColor.Gray);
                sb.AppendLineF($"DMG: {source.StatSheet.Dmg} ");
                sb.AppendLineF($"HIT: {source.StatSheet.Hit} ");
                sb.AppendLineF($"Attack Speed%: {source.StatSheet.AtkSpeedPct} ");
                sb.AppendLineF($"Flat Skill DMG: {source.StatSheet.FlatSkillDamage} ");
                sb.AppendLineF($"Skill DMG%: {source.StatSheet.SkillDamagePct} ");
                sb.AppendLineF($"Flat Spell DMG: {source.StatSheet.FlatSpellDamage} ");
                sb.AppendLineF($"Spell DMG%: {source.StatSheet.SpellDamagePct} ");

                source.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
                Subject.Close(source);

                break;
            }
            case "terminus_checkgearedstats":
            {
                var sb = new StringBuilder();

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Vitality", MessageColor.Gray);
                sb.Append($"HP: {source.StatSheet.EffectiveMaximumHp} ");
                sb.AppendLine($"MP: {source.StatSheet.EffectiveMaximumMp} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Stats", MessageColor.Gray);
                sb.Append($"STR: {source.StatSheet.EffectiveStr} ");
                sb.Append($"INT: {source.StatSheet.EffectiveInt} ");
                sb.Append($"WIS: {source.StatSheet.EffectiveWis} ");
                sb.Append($"CON: {source.StatSheet.EffectiveCon} ");
                sb.AppendLine($"DEX: {source.StatSheet.EffectiveDex} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Defenses", MessageColor.Gray);
                sb.Append($"MR: {source.StatSheet.EffectiveMagicResistance}% ");
                sb.AppendLine($"AC: {source.StatSheet.EffectiveAc} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Offenses", MessageColor.Gray);
                sb.AppendLineF($"DMG: {source.StatSheet.EffectiveDmg} ");
                sb.AppendLineF($"HIT: {source.StatSheet.EffectiveHit} ");
                sb.AppendLineF($"Attack Speed%: {source.StatSheet.EffectiveAttackSpeedPct} ");
                sb.AppendLineF($"Flat Skill DMG: {source.StatSheet.EffectiveFlatSkillDamage} ");
                sb.AppendLineF($"Skill DMG%: {source.StatSheet.EffectiveSkillDamagePct} ");
                sb.AppendLineF($"Flat Spell DMG: {source.StatSheet.EffectiveFlatSpellDamage} ");
                sb.AppendLineF($"Spell DMG%: {source.StatSheet.EffectiveSpellDamagePct} ");

                source.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
                Subject.Close(source);

                break;
            }
        }
    }
}