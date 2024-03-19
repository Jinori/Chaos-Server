using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts;

public class EquipmentScript(Item subject) : ConfigurableItemScriptBase(subject)
{
    private void EquipStaff(Aisling source, ItemTemplate template)
    {
        if (template.EquipmentType is not null)
            source.Equip(template.EquipmentType.Value, Subject);
    }
    
    private bool CanWieldStaff(Aisling source, string skillName)
    {
        if (source.UserStatSheet.BaseClass == BaseClass.Priest)
            return true;
        
        if (!source.SkillBook.Contains(skillName))
        {
            source.SendOrangeBarMessage($"You do not have the skill to wield it.");
            return false;
        }
        return true;
    }

    public override void OnUse(Aisling source)
    {
        var template = Subject.Template;

        if (template.Category.Contains("Staff"))
        {
            // Check specific conditions for equipping a staff
            if (template.TemplateKey.ContainsI("magus") && CanWieldStaff(source, "Wield Magus Staff"))
            {
                if (source.Equipment.Contains((byte)EquipmentSlot.Shield)
                    && source.Equipment.TryGetObject((byte)EquipmentSlot.Shield, out var shield))
                {
                    source.SendOrangeBarMessage($"You cannot equip a staff while using {shield.DisplayName}.");
                    return;
                }

                EquipStaff(source, template);
                return;
            }

            if (template.TemplateKey.ContainsI("holy") && CanWieldStaff(source, "Wield Holy Staff"))
            {
                if (source.Equipment.Contains((byte)EquipmentSlot.Shield)
                    && source.Equipment.TryGetObject((byte)EquipmentSlot.Shield, out var shield))
                {
                    source.SendOrangeBarMessage($"You cannot equip a staff while using {shield.DisplayName}.");
                    return;
                }

                EquipStaff(source, template);
                return;
            }
        }

        if (template.Category.ContainsI("2H"))
        {
            // Ensure the character is not already equipped with a staff
            if (source.Equipment.TryGetObject((byte)EquipmentSlot.Shield, out var item))
            {
                if (item.Template.Category.ContainsI("Shield"))
                {
                    source.SendOrangeBarMessage("You cannot equip a two handed weapon with a shield equipped."); 
                    return;   
                }
            }
        }
        
        // Check if the item is a shield
        if (template.Category.Contains("Shield"))
        {
            // Ensure the character is not already equipped with a staff
            if (source.Equipment.TryGetObject((byte)EquipmentSlot.Weapon, out var item))
            {
                if (item.Template.Category.ContainsI("Staff"))
                {
                    source.SendOrangeBarMessage("You cannot equip a shield while having a staff equipped."); 
                    return;   
                }

                if (item.Template.Category.ContainsI("2H"))
                {
                    source.SendOrangeBarMessage("You cannot equip a shield while wielding a two handed weapon."); 
                    return;
                }
            }
        }
        
        if (template.EquipmentType is null or EquipmentType.NotEquipment)
        {
            source.SendOrangeBarMessage("You can't equip that");

            return;
        }

        //gender check
        if (template.Gender.HasValue && !template.Gender.Value.HasFlag(source.Gender))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you");

            return;
        }

        if (template.Class.HasValue && !source.HasClass(template.Class.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} is not for {source.UserStatSheet.BaseClass}.");

            return;
        }

        if (template.AdvClass.HasValue && (template.AdvClass.Value != source.UserStatSheet.AdvClass))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} is not for {source.UserStatSheet.AdvClass}.");

            return;
        }

        if (template.Level > source.UserStatSheet.Level)
        {
            source.SendOrangeBarMessage($"You are too inexperienced to equip {Subject.Template.Name}.");

            return;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} does not seem to fit you, but you could grow into it");

            return;
        }

        if (StatRequired.HasValue
            && StatAmountRequired.HasValue
            && (source.StatSheet.GetBaseStat(StatRequired.Value) < StatAmountRequired.Value))
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} requires {StatAmountRequired.Value} {StatRequired} to equip.");

            return;
        }

        if (template.RequiresMaster && !source.UserStatSheet.Master)
        {
            source.SendOrangeBarMessage($"{Subject.DisplayName} requires Master to be able to be equipped.");

            return;
        }

        source.Equip(template.EquipmentType.Value, Subject);
    }

    #region ScriptVars
    protected int? StatAmountRequired { get; init; }
    protected Stat? StatRequired { get; init; }
    #endregion
}