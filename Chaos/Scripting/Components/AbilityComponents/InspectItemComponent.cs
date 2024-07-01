using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;
using Chaos.Collections.Common;
using Chaos.Models.Panel;
using System.Text;
using Chaos.Extensions.Common;

namespace Chaos.Scripting.Components.AbilityComponents
{
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
                    "Drink" => BuildDrinkItemMessage(item),
                    _        => BuildGenericItemMessage(item)
                };

                context.SourceAisling?.SendServerMessage(options.OutputType.Value, message);
            }
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

            if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable) && vitalityConsumable is DynamicVars vitalityData)
                sb.Append(ExtractVitalityData(vitalityData));

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

            if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable) && vitalityConsumable is DynamicVars vitalityData)
                sb.Append(ExtractVitalityData(vitalityData));

            return sb.ToString();
        }

        private string BuildFishItemMessage(Item item)
        {
            var sb = new StringBuilder();
            sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Fish]\n"))
              .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
              .AppendLineF(CenterText($"Weight: {item.Template.Weight}"))
              .AppendLineF(CenterText($"Buy Cost: {item.Template.BuyCost}"))
              .AppendLineF(CenterText($"Sell Value: {item.Template.SellValue}"))
              .AppendLineF(CenterText($"Max Stack: {item.Template.MaxStacks}"));

            if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable) && vitalityConsumable is DynamicVars vitalityData)
                sb.Append(ExtractVitalityData(vitalityData));

            return sb.ToString();
        }

        private string BuildFoodItemMessage(Item item)
        {
            var sb = new StringBuilder();
            sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Food]\n"))
              .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
              .AppendLineF(CenterText($"Weight: {item.Template.Weight}"))
              .AppendLineF(CenterText($"Buy Cost: {item.Template.BuyCost}"))
              .AppendLineF(CenterText($"Sell Value: {item.Template.SellValue}"))
              .AppendLineF(CenterText($"Max Stack: {item.Template.MaxStacks}"));

            if (item.Template.ScriptVars.TryGetValue("VitalityConsumable", out var vitalityConsumable) && vitalityConsumable is DynamicVars vitalityData)
                sb.Append(ExtractVitalityData(vitalityData));

            return sb.ToString();
        }

        private string BuildGenericItemMessage(Item item)
        {
            var sb = new StringBuilder();
            sb.AppendLineFColored(MessageColor.Orange, CenterText("[Inspect Item]\n"))
              .AppendLineFColored(MessageColor.Gainsboro, CenterText($"Name: {item.DisplayName}"))
              .AppendLineF(CenterText($"Level: {item.Level}"))
              .AppendLineF(CenterText($"Weight: {item.Template.Weight}"))
              .AppendLineF(CenterText($"AC: {item.Template.Modifiers?.Ac}"))
              .AppendLineF(CenterText($"Buy Cost: {item.Template.BuyCost}"))
              .AppendLineF(CenterText($"Sell Value: {item.Template.SellValue}"))
              .AppendLineF(CenterText($"Max Stack: {item.Template.MaxStacks}"))
              .AppendLineF(CenterText($"Skill Damage: {item.Modifiers.FlatSkillDamage}"))
              .AppendLineF(CenterText($"Spell Damage: {item.Modifiers.FlatSpellDamage}"))
              .AppendLineF(CenterText($"Health: {item.Modifiers.MaximumHp}"))
              .AppendLineF(CenterText($"Mana: {item.Modifiers.MaximumMp}"))
              .AppendLineF(CenterText($"Attack Speed %: {item.Modifiers.AtkSpeedPct}"))
              .AppendLineF(CenterText($"Magic Resistance: {item.Modifiers.MagicResistance}"))
              .AppendLineF(CenterText($"Strength: {item.Modifiers.Str}"))
              .AppendLineF(CenterText($"Intelligence: {item.Modifiers.Int}"))
              .AppendLineF(CenterText($"Wisdom: {item.Modifiers.Wis}"))
              .AppendLineF(CenterText($"Constitution: {item.Modifiers.Con}"))
              .AppendLineF(CenterText($"Dexterity: {item.Modifiers.Dex}"))
              .AppendLineF(CenterText($"DMG: {item.Modifiers.Dmg}"))
              .AppendLineF(CenterText($"HIT: {item.Modifiers.Hit}"))
              .AppendLineF(CenterText($"Skill Pct Damage: {item.Modifiers.SkillDamagePct}"))
              .AppendLineF(CenterText($"Spell Pct Damage: {item.Modifiers.SpellDamagePct}"));

            return sb.ToString();
        }

        private string ExtractVitalityData(DynamicVars vitalityData)
        {
            var sb = new StringBuilder();

            AppendVitalityValue(sb, vitalityData, "baseDamage", "Vitality - Damage");
            AppendVitalityValue(sb, vitalityData, "PctHpDamage", "Vitality - Damage %");
            AppendVitalityValue(sb, vitalityData, "baseHeal", "Vitality - Heal");
            AppendVitalityValue(sb, vitalityData, "PctHpHeal", "Vitality - Heal %");
            AppendVitalityValue(sb, vitalityData, "manaReplenish", "Vitality - Mana");
            AppendVitalityValue(sb, vitalityData, "PctManaReplenish", "Vitality - Mana %");
            AppendVitalityValue(sb, vitalityData, "manaDrain", "Vitality - Mana Drain");
            AppendVitalityValue(sb, vitalityData, "PctManaDrain", "Vitality - Mana Drain %");

            return sb.ToString();
        }

        private void AppendVitalityValue(StringBuilder sb, DynamicVars vitalityData, string key, string label)
        {
            int? value = vitalityData.Get<int>(key);
            if (value.HasValue && (value.Value != 0))
                sb.AppendLineF(CenterText($"{label}: {value.Value}"));
        }

        private string CenterText(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            var padding = (MAX_LINE_WIDTH - text.Length) / 2;
            if (padding > 0)
                return new string(' ', padding) + text;

            return text;
        }

        private string CenterDescription(string? text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            text = text.Replace("\n", " ").Replace("\r", " ");

            var words = text.Split(' ');
            var sb = new StringBuilder();
            var line = new StringBuilder();

            foreach (var word in words)
            {
                if (line.Length + word.Length + 1 > MAX_LINE_WIDTH)
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

        public interface IInspectItemComponentOptions
        {
            ServerMessageType? OutputType { get; init; }
        }
    }

    internal class InspectItemComponentImpl : InspectItemComponent { }
}
