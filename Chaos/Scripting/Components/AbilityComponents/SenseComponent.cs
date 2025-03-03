using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public sealed class SenseComponent : IComponent
{
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ISenseComponentOptions>();
        var targets = vars.GetTargets<Aisling>();

        foreach (var target in targets)
        {
            if (options.OutputType != null)
            {
                var sb = new StringBuilder();
                var vitality = target.UserStatSheet.MaximumHp + target.UserStatSheet.MaximumMp * 2;
                sb.AppendLineFColored(MessageColor.Silver, $"{target.Name}'s Stats");
                sb.Append(Environment.NewLine);
                
                sb.AppendLineFColored(MessageColor.Yellow, "Base Vitality", MessageColor.Gray);
                sb.Append($"Vitality: {vitality} ");
                sb.Append($"HP: {target.StatSheet.MaximumHp} ");
                sb.AppendLine($"MP: {target.StatSheet.MaximumMp} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Base Stats", MessageColor.Gray);
                sb.Append($"STR: {target.StatSheet.Str} ");
                sb.Append($"INT: {target.StatSheet.Int} ");
                sb.Append($"WIS: {target.StatSheet.Wis} ");
                sb.Append($"CON: {target.StatSheet.Con} ");
                sb.AppendLine($"DEX: {target.StatSheet.Dex} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Base Defenses", MessageColor.Gray);
                sb.Append($"MR: {target.StatSheet.MagicResistance}% ");
                sb.AppendLine($"AC: {target.StatSheet.Ac} ");
                sb.AppendLine($"Heal Bonus: {target.StatSheet.HealBonus} ");
                sb.AppendLine($"Heal Bonus Pct: {target.StatSheet.HealBonusPct} ");
                sb.AppendLine($"Cooldown Reduction: {target.StatSheet.CooldownReduction} ");
                sb.AppendLine($"Cooldown Reduction Pct: {target.StatSheet.CooldownReductionPct} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Base Offenses", MessageColor.Gray);
                sb.AppendLineF($"DMG: {target.StatSheet.Dmg} ");
                sb.AppendLineF($"HIT: {target.StatSheet.Hit} ");
                sb.AppendLineF($"Attack Speed%: {target.StatSheet.AtkSpeedPct} ");
                sb.AppendLineF($"Flat Skill DMG: {target.StatSheet.FlatSkillDamage} ");
                sb.AppendLineF($"Skill DMG%: {target.StatSheet.SkillDamagePct} ");
                sb.AppendLineF($"Flat Spell DMG: {target.StatSheet.FlatSpellDamage} ");
                sb.AppendLineF($"Spell DMG%: {target.StatSheet.SpellDamagePct} ");

                sb.AppendLine("");
                
                var gearedvitality = target.UserStatSheet.EffectiveMaximumHp + target.UserStatSheet.EffectiveMaximumMp * 2;

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Vitality", MessageColor.Gray);
                sb.Append($"Vitality: {gearedvitality} ");
                sb.Append($"HP: {target.StatSheet.EffectiveMaximumHp} ");
                sb.AppendLine($"MP: {target.StatSheet.EffectiveMaximumMp} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Stats", MessageColor.Gray);
                sb.Append($"STR: {target.StatSheet.EffectiveStr} ");
                sb.Append($"INT: {target.StatSheet.EffectiveInt} ");
                sb.Append($"WIS: {target.StatSheet.EffectiveWis} ");
                sb.Append($"CON: {target.StatSheet.EffectiveCon} ");
                sb.AppendLine($"DEX: {target.StatSheet.EffectiveDex} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Defenses", MessageColor.Gray);
                sb.Append($"MR: {target.StatSheet.EffectiveMagicResistance}% ");
                sb.AppendLine($"AC: {target.StatSheet.EffectiveAc} ");
                sb.AppendLine($"Heal Bonus: {target.StatSheet.EffectiveHealBonus} ");
                sb.AppendLine($"Heal Bonus Pct: {target.StatSheet.EffectiveHealBonusPct} ");
                sb.AppendLine($"Cooldown Reduction: {target.StatSheet.EffectiveCooldownReduction} ");
                sb.AppendLine($"Cooldown Reduction Pct: {target.StatSheet.EffectiveCooldownReductionPct} ");

                sb.AppendLineFColored(MessageColor.Yellow, "Geared Offenses", MessageColor.Gray);
                sb.AppendLineF($"DMG: {target.StatSheet.EffectiveDmg} ");
                sb.AppendLineF($"HIT: {target.StatSheet.EffectiveHit} ");
                sb.AppendLineF($"Attack Speed%: {target.StatSheet.EffectiveAttackSpeedPct} ");
                sb.AppendLineF($"Flat Skill DMG: {target.StatSheet.EffectiveFlatSkillDamage} ");
                sb.AppendLineF($"Skill DMG%: {target.StatSheet.EffectiveSkillDamagePct} ");
                sb.AppendLineF($"Flat Spell DMG: {target.StatSheet.EffectiveFlatSpellDamage} ");
                sb.AppendLineF($"Spell DMG%: {target.StatSheet.EffectiveSpellDamagePct} ");
                
                context.SourceAisling?.Client.SendServerMessage(ServerMessageType.ScrollWindow, sb.ToString());
            }
        }
    }

    public interface ISenseComponentOptions
    {
        ServerMessageType? OutputType { get; init; }
    }
}
