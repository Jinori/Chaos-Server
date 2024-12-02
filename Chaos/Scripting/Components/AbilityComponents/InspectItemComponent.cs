using System.Text;
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public class InspectItemComponent : IComponent
{
    private const int MAX_LINE_WIDTH = 50;

    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var item = context.SourceAisling?.Inventory.FirstOrDefault();
        var options = vars.GetOptions<IInspectItemComponentOptions>();

        if (item is null)
        {
            context.SourceAisling?.SendActiveMessage("Put an item in the first slot of your inventory.");

            return;
        }

        if (options.OutputType != null)
        {
            var message = item.Template.Category switch
            {
                "Food"   => BuildFoodItemMessage(item),
                "Fish"   => BuildFishItemMessage(item),
                "Potion" => BuildPotionItemMessage(item),
                "Drink"  => BuildDrinkItemMessage(item),
                _        => BuildGenericItemMessage(item)
            };

            context.SourceAisling?.SendServerMessage(options.OutputType.Value, message);
        }
    }

    private void AppendPropertyIfNotNullOrEmpty(StringBuilder sb, string propertyName, string? value)
    {
        if (!string.IsNullOrEmpty(value))
            sb.AppendLineF(CenterText($"{propertyName}: {value}"));
    }

    private void AppendPropertyIfNotZero(StringBuilder sb, string propertyName, int value)
    {
        if (value != 0)
            sb.AppendLineF(CenterText($"{propertyName}: {value}"));
    }

    private void AppendVitalityValue(
        StringBuilder sb,
        DynamicVars vitalityData,
        string key,
        string label)
    {
        int? value = vitalityData.Get<int>(key);

        if (value.HasValue && (value.Value != 0))
            sb.AppendLineF(CenterText($"{label}: {value.Value}"));
    }

    private string BuildDrinkItemMessage(Item item)
    {
        var sb = new StringBuilder();

        sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Drink]\n"))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
          .AppendLineFColored(MessageColor.Yellow, CenterDescription(item.Template.Description))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Weight: {item.Template.Weight}"))
          .AppendLineF(CenterText($"Buy Cost: {item.Template.BuyCost}"))
          .AppendLineF(CenterText($"Sell Value: {item.Template.SellValue}"))
          .AppendLineF(CenterText($"Max Stack: {item.Template.MaxStacks}"));

        if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable)
            && vitalityConsumable is DynamicVars vitalityData)
            sb.Append(ExtractVitalityData(vitalityData));

        return sb.ToString();
    }

    private string BuildFishItemMessage(Item item)
    {
        var sb = new StringBuilder();

        sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Fish]\n"))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
          .AppendLineFColored(MessageColor.Yellow, CenterDescription(item.Template.Description))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Weight: {item.Template.Weight}"))
          .AppendLineF(CenterText($"Buy Cost: {item.Template.BuyCost}"))
          .AppendLineF(CenterText($"Sell Value: {item.Template.SellValue}"))
          .AppendLineF(CenterText($"Max Stack: {item.Template.MaxStacks}"));

        if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable)
            && vitalityConsumable is DynamicVars vitalityData)
            sb.Append(ExtractVitalityData(vitalityData));

        return sb.ToString();
    }

    private string BuildFoodItemMessage(Item item)
    {
        var sb = new StringBuilder();

        sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Food]\n"))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
          .AppendLineFColored(MessageColor.Yellow, CenterDescription(item.Template.Description))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Weight: {item.Template.Weight}"))
          .AppendLineF(CenterText($"Buy Cost: {item.Template.BuyCost}"))
          .AppendLineF(CenterText($"Sell Value: {item.Template.SellValue}"))
          .AppendLineF(CenterText($"Max Stack: {item.Template.MaxStacks}"));

        if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable)
            && vitalityConsumable is DynamicVars vitalityData)
            sb.Append(ExtractVitalityData(vitalityData));

        return sb.ToString();
    }

    private string BuildGenericItemMessage(Item item)
    {
        var sb = new StringBuilder();

        sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Item]\n"))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
          .AppendLineFColored(MessageColor.Yellow, CenterDescription(item.Template.Description))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Level: {item.Template.Level}"));
        AppendPropertyIfNotNullOrEmpty(sb, "Gender", item.Template.Gender.ToString());
        AppendPropertyIfNotNullOrEmpty(sb, "Class", item.Template.Class.ToString());
        AppendPropertyIfNotZero(sb, "Weight", item.Template.Weight);
        AppendPropertyIfNotZero(sb, "AC", item.Modifiers.Ac);
        AppendPropertyIfNotZero(sb, "Buy Cost", item.Template.BuyCost);
        AppendPropertyIfNotZero(sb, "Sell Value", item.Template.SellValue);
        AppendPropertyIfNotZero(sb, "Max Stack", item.Template.MaxStacks);
        AppendPropertyIfNotZero(sb, "Skill Damage", item.Modifiers.FlatSkillDamage);
        AppendPropertyIfNotZero(sb, "Spell Damage", item.Modifiers.FlatSpellDamage);
        AppendPropertyIfNotZero(sb, "Health", item.Modifiers.MaximumHp);
        AppendPropertyIfNotZero(sb, "Mana", item.Modifiers.MaximumMp);
        AppendPropertyIfNotZero(sb, "Attack Speed %", item.Modifiers.AtkSpeedPct);
        AppendPropertyIfNotZero(sb, "Magic Resistance", item.Modifiers.MagicResistance);
        AppendPropertyIfNotZero(sb, "Strength", item.Modifiers.Str);
        AppendPropertyIfNotZero(sb, "Intelligence", item.Modifiers.Int);
        AppendPropertyIfNotZero(sb, "Wisdom", item.Modifiers.Wis);
        AppendPropertyIfNotZero(sb, "Constitution", item.Modifiers.Con);
        AppendPropertyIfNotZero(sb, "Dexterity", item.Modifiers.Dex);
        AppendPropertyIfNotZero(sb, "DMG", item.Modifiers.Dmg);
        AppendPropertyIfNotZero(sb, "HIT", item.Modifiers.Hit);
        AppendPropertyIfNotZero(sb, "Skill Pct Damage", item.Modifiers.SkillDamagePct);
        AppendPropertyIfNotZero(sb, "Spell Pct Damage", item.Modifiers.SpellDamagePct);
        AppendPropertyIfNotZero(sb, "Heal Bonus", item.Modifiers.HealBonus);
        AppendPropertyIfNotZero(sb, "Heal Bonus Pct", item.Modifiers.HealBonusPct);
        AppendPropertyIfNotZero(sb, "Cooldown Reduction", item.Modifiers.CooldownReduction);
        AppendPropertyIfNotZero(sb, "Cooldown Reduction Pct", item.Modifiers.CooldownReductionPct);

        return sb.ToString();
    }

    private string BuildPotionItemMessage(Item item)
    {
        var sb = new StringBuilder();

        sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Potion]\n"))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
          .AppendLineFColored(MessageColor.Yellow, CenterDescription(item.Template.Description))
          .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Weight: {item.Template.Weight}"))
          .AppendLineF(CenterText($"Buy Cost: {item.Template.BuyCost}"))
          .AppendLineF(CenterText($"Sell Value: {item.Template.SellValue}"))
          .AppendLineF(CenterText($"Max Stack: {item.Template.MaxStacks}"));

        if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable)
            && vitalityConsumable is DynamicVars vitalityData)
            sb.Append(ExtractVitalityData(vitalityData));

        return sb.ToString();
    }

    private string CenterDescription(string? text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        text = text.Replace("\n", " ")
                   .Replace("\r", " ");

        var words = text.Split(' ');
        var sb = new StringBuilder();
        var line = new StringBuilder();

        foreach (var word in words)
        {
            if ((line.Length + word.Length + 1) > MAX_LINE_WIDTH)
            {
                sb.AppendLine(CenterText(line.ToString()));
                line.Clear();
            }

            if (line.Length > 0)
                line.Append(' ');

            line.Append(word);
        }

        if (line.Length > 0)
            sb.AppendLine(CenterText(line.ToString()));

        return sb.ToString();
    }

    private string CenterText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var padding = (MAX_LINE_WIDTH - text.Length) / 2;

        if (padding > 0)
            return new string(' ', padding) + text;

        return text;
    }

    private string ExtractVitalityData(DynamicVars vitalityData)
    {
        var sb = new StringBuilder();

        AppendVitalityValue(
            sb,
            vitalityData,
            "baseDamage",
            "Vitality - Damage");

        AppendVitalityValue(
            sb,
            vitalityData,
            "PctHpDamage",
            "Vitality - Damage %");

        AppendVitalityValue(
            sb,
            vitalityData,
            "baseHeal",
            "Vitality - Heal");

        AppendVitalityValue(
            sb,
            vitalityData,
            "PctHpHeal",
            "Vitality - Heal %");

        AppendVitalityValue(
            sb,
            vitalityData,
            "manaReplenish",
            "Vitality - Mana");

        AppendVitalityValue(
            sb,
            vitalityData,
            "PctManaReplenish",
            "Vitality - Mana %");

        AppendVitalityValue(
            sb,
            vitalityData,
            "manaDrain",
            "Vitality - Mana Drain");

        AppendVitalityValue(
            sb,
            vitalityData,
            "PctManaDrain",
            "Vitality - Mana Drain %");

        return sb.ToString();
    }

    public interface IInspectItemComponentOptions
    {
        ServerMessageType? OutputType { get; init; }
    }
}

internal class InspectItemComponentImpl : InspectItemComponent { }